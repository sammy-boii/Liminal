using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldElevator {
    public class DoorTrigger : MonoBehaviour {
        public ElevatorBrain brain;
        public bool isFullyClosed = true;
        public bool isFullyOpen = false;
        public enum DoorType { Accordion, Hinged }
        public DoorType doorType = DoorType.Hinged;
        public enum DoorLocation { Internal, External }
        public DoorLocation doorLocation = DoorLocation.External;

        public enum MovementType { Normal, CustomCurve}
        public MovementType movementType = MovementType.Normal;
        public AnimationCurve angleCurve;
        public bool isRightDoor = false;
        private float angleCurveStartTime;
        private float curveSpeed = 1f;

        public bool interactWithBoth = true;

        public DoorTrigger otherPaneOfThisPair;
        private SkinnedMeshRenderer _smr;
        private bool opening = false;
        private bool closing = false;
        [Range(.1f, .95f)] public float howFast = .1f;
        private float currentSpeed;
        public bool randomizeHowFast = true;
        private float currentWeight = 0;
        public bool locked = true;

        public Transform floorStop;


        public float destinationZAngle;
        public float currentZAngle;
        private Vector3 originalLocalRotation;
        [SerializeField] private float openAngle = 100f;
        [SerializeField] private float closedAngle = 0f;


        [SerializeField] private List<AudioSource> audioDoorOpening = new List<AudioSource>();
        [SerializeField] private List<AudioSource> audioDoorClosing = new List<AudioSource>();

        void Awake() {
            if (doorType == DoorType.Accordion) {
                TryGetComponent(out _smr);
                RebakeMesh();
            }
        }

        void Start() {
            //each door reports itself to the brain
            if (doorLocation == DoorLocation.External) brain.outterDoors.Add(this);
            if (doorLocation == DoorLocation.Internal) brain.innerDoors.Add(this);

            originalLocalRotation = transform.localEulerAngles;

            //get sounds
            Transform openingFolder = transform.Find("Opening Sounds");
            if (openingFolder != null)
                foreach (AudioSource audio in openingFolder.GetComponentsInChildren<AudioSource>()) 
                    audioDoorOpening.Add(audio);

            Transform closingFolder = transform.Find("Closing Sounds");
            if (closingFolder != null)
                foreach (AudioSource audio in closingFolder.GetComponentsInChildren<AudioSource>())
                    audioDoorClosing.Add(audio);
        }


        void FixedUpdate() {
            if (doorType == DoorType.Accordion) {
                if (opening) {
                    currentWeight = Mathf.Lerp(currentWeight, 100, currentSpeed);
                    _smr.SetBlendShapeWeight(0, currentWeight);
                    if (currentWeight > 99f) OpeningEnds();
                }

                if (closing) {
                    currentWeight = Mathf.Lerp(currentWeight, 0, currentSpeed);
                    _smr.SetBlendShapeWeight(0, currentWeight);
                    if (currentWeight < 1f) ClosingEnds();
                }
            }

            if (doorType == DoorType.Hinged && (opening || closing)) {
                if (movementType == MovementType.Normal) {
                    currentZAngle = Mathf.Lerp(currentZAngle, destinationZAngle, .1f);



                    if (Mathf.Abs(currentZAngle - destinationZAngle) < .1f) {
                        if (opening) OpeningEnds();
                        if (closing) ClosingEnds();
                    }
                } else {
                    float elapsedTime = (Time.time - angleCurveStartTime) * curveSpeed;
                    currentZAngle = angleCurve.Evaluate(elapsedTime) * (isRightDoor ? -1 : 1);
                    if (currentZAngle <.1f && !isRightDoor && elapsedTime > 1f) ClosingEnds();
                    if (currentZAngle >-.1f && isRightDoor && elapsedTime > 1f) ClosingEnds();

                }
                transform.localEulerAngles = new Vector3(
                    originalLocalRotation.x,
                    originalLocalRotation.y,
                    currentZAngle
                );
            }
        }

        private void ClosingEnds() {
            isFullyClosed = true;
            closing = false;
            opening = false;
            if (doorType == DoorType.Accordion) RebakeMesh();

            if (doorLocation == DoorLocation.Internal) brain.InnerDoorsChangeState();

            StopAudio(audioDoorOpening);
            RandomizeAndPlay(audioDoorClosing);
        }

        private void OpeningEnds() {
            isFullyOpen = true;
            opening = false;
            if (doorType == DoorType.Accordion) RebakeMesh();
            //if (doorLocation == DoorLocation.Internal) brain.InnerDoorsChangeState();

            if (movementType == MovementType.Normal) {
                StopAudio(audioDoorOpening);
                RandomizeAndPlay(audioDoorClosing);
            }
            if (doorType == DoorType.Accordion) GetComponent<MeshCollider>().enabled = true;
        }

        

        [ContextMenu("Open")]
        public void Open() {
            if (!locked) {
                opening = true;
                closing = false;
                isFullyClosed = false;
                isFullyOpen = false;
                currentSpeed = randomizeHowFast ? Random.Range(.03f, .15f) : howFast;
                if (doorLocation == DoorLocation.Internal) brain.InnerDoorsChangeState();
                DoorStartsOpening();

                if (doorType == DoorType.Hinged) destinationZAngle = openAngle;

                if (interactWithBoth && otherPaneOfThisPair.isFullyClosed) otherPaneOfThisPair.Open();
                angleCurveStartTime = Time.time;
                curveSpeed = Random.Range(.9f, 1.2f);

                if (doorType == DoorType.Accordion) GetComponent<MeshCollider>().enabled = false;
            }
        }


        [ContextMenu("Close")]
        public void Close(bool slowly = false) {
            if (!locked) {
                opening = false;
                closing = true;
                isFullyClosed = false;
                isFullyOpen = false;
                currentSpeed = randomizeHowFast ? Random.Range(.1f, .2f) : howFast;
                if (slowly) currentSpeed *= .1f;
                if (doorLocation == DoorLocation.Internal) brain.InnerDoorsChangeState();
                DoorStartsClosing();
                if (doorType == DoorType.Hinged) destinationZAngle = closedAngle;
                if (interactWithBoth && !otherPaneOfThisPair.closing) otherPaneOfThisPair.Close();
            }
        }

        public void Toggle() {
            if (isFullyClosed) Open();
            //if (isFullyOpen) 
            else Close();

        }

        void RebakeMesh() {
            MeshCollider collider = GetComponent<MeshCollider>();
            if (collider != null) {
                Mesh bakeMesh = new Mesh();
                _smr.BakeMesh(bakeMesh);
                collider.sharedMesh = bakeMesh;
            }
        }

        public void DoorStartsOpening() {
            RandomizeAndPlay(audioDoorOpening);
        }

        public void DoorStartsClosing() {
            if (movementType == MovementType.Normal) {
                RandomizeAndPlay(audioDoorOpening);
            }
        }



        void RandomizeAndPlay(List<AudioSource> audioList) {
            if (audioList.Count == 0) return;
            AudioSource _audio = audioList[Random.Range(0, audioList.Count)];
            _audio.pitch = Random.Range(.8f, 1.3f);
            //_audio.volume = Random.Range(.8f, 1.3f );
            _audio.Play();
        }

        void StopAudio(List<AudioSource> audioList) {
            if (audioList.Count == 0) return;
            foreach (AudioSource _audio in audioList) {
                if (_audio.isPlaying) _audio.Stop();
            }
        }
    }
}