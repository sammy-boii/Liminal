using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldElevator {
    [DefaultExecutionOrder(1100)] //after DoorTrigger
    public class ElevatorBrain : MonoBehaviour {


        [Space(10)]
        [Header("Speed ____________________________________________________________")]
        [Space(40)]
        [SerializeField] private float unitsPerSecond = 1.5f;
        [SerializeField] private bool slowDownNearArriving = true;
        [Range(.01f, .03f)] [SerializeField] private float slowingDownEffect = .02f;
        public Transform initialStop;
        [HideInInspector] public Vector3 destination;
        [HideInInspector] public List<DoorTrigger> outterDoors = new List<DoorTrigger>();
        [HideInInspector] public List<DoorTrigger> innerDoors = new List<DoorTrigger>();



        [Space(10)]
        [Header("Avoid player bouncing while traveling _________________________")]
        [Space(40)]
        [SerializeField] private bool avoidBouncing = true;
        public Transform player;
        [SerializeField] private float distToFloor = 1f;




        [Space(10)]
        [Header("Safety measures ________________________________________________")]
        [Space(40)]
        [SerializeField] private bool outterDoorsMustBeFullyClosed = true;
        [SerializeField] private bool innerDoorsMustBeFullyClosed = true;
        [SerializeField] private bool lockOutterDoorsInOtherFloors = true;
        [SerializeField] private bool emergencyStopIfInnerDoorsOpen = true;
        [SerializeField] private bool resumeTravelAfterDoorsClose = true;




        [Space(10)]
        [Header("Not everything is pink _________________________________________")]
        [Space(40)]
        [SerializeField] private FlickeringFrecuency lightFlickering = FlickeringFrecuency.AlmostUnnoticeable;
        public enum FlickeringFrecuency { NoFlickering, AlmostUnnoticeable, Annoying, Heavy, Nightmare }

        [SerializeField] private ElevatorShaking elevatorShaking = ElevatorShaking.AlmostUnnoticeable;
        public enum ElevatorShaking { NoShaking, AlmostUnnoticeable, Annoying, Heavy, Nightmare }

        [Range(0,2f)] [SerializeField] private float shakeWhenTravelBegins = 1f;
        [Range(0,2f)] [SerializeField] private float shakeWhenTravelEnds = .5f;
        [Range(0,2f)] [SerializeField] private float shakeOnEmergencyStop = 1.2f;




        [Space(10)]
        [Header("Internal references ________________________________________________")]
        [Space(40)]
        public Transform elevator;
        public GameObject elevatorLightsContainer;
        public ElevatorButton elevatorLightSwitchScript;




        [Space(10)]
        [Header("Lights Materials ________________________________________")]
        [Space(40)]
        [SerializeField] private Material isMovingLight;
        [SerializeField] private Material movingUpLight;
        [SerializeField] private Material movingDownLight;
        [SerializeField] private Color movingLightColor = Color.green;
        [SerializeField] private Material emergencyStopLight;
        [SerializeField] private Color emergencyStopColor = Color.red;
        [SerializeField] private float callerLightsIntensity = 20f;





        [Space(10)]
        [Header("Sounds _________________________________________________________")]
        [Space(40)]
        public AudioSource travelSound;
        public AudioSource stopSound;
        public AudioSource emergencyStopSound;
        public AudioSource doorUnlockedSound;
        public AudioSource breaksSound;
        public AudioSource[] flickeringSounds = new AudioSource[3];






        //non-exposed variables
        private float nextFlicker = 1f;
        private float nextShaking = 1f;
        private float currentShakeMagnitude = 0;
        private float multiplier = 2f;
        private Quaternion originalElevatorRotation;
        private float currentSpeed;
        [HideInInspector] public bool moving = false;
        private int direction;
        private bool breaking = false;
        private bool hasShownPlayerWarning = false;
        private Transform stopsContainer;
        private bool lightsState = true;


        public void GoToFloor(int floor) {      // <- main verb
            GoToPosition(stopsContainer.GetChild(floor).position);
        }

        public void GoToPosition(Vector3 pos) {
            if (SafetyCheck()) {
                //don't go if moving in the opposite direction
                if (moving && direction != Mathf.Sign(pos.y - elevator.position.y)) return;

                //ok, go
                destination = pos;
                if (!ElevatorIsOnDestination()) {
                    if (!moving) {
                        travelSound.volume = 1f;
                        travelSound.pitch = 1f;
                        travelSound.Play();
                        currentShakeMagnitude = shakeWhenTravelBegins;
                    }
                    direction = (int)Mathf.Sign(destination.y - elevator.position.y);
                    currentSpeed = unitsPerSecond / 50f;
                    moving = true;
                    
                }
            }
            UpdateCallersLights();
        }

        public void ElevatorLightsToggle(bool state) {
            lightsState = state;
            CancelInvoke("EmergencyStopEnds");
                       
        }



        public void EmergencyStopBegin() {
            if (moving) {
                breaksSound.volume = 1;
                breaksSound.pitch = 1.5f;
                breaksSound.Play();
                
                travelSound.volume *= .8f;
                travelSound.pitch = .9f;

                currentSpeed = currentSpeed * .3f;
                Invoke("EmergencyStopEnds", 1.2f);
                currentShakeMagnitude = shakeOnEmergencyStop;

                //red light
                emergencyStopLight.SetColor("_EmissiveColor", emergencyStopColor * callerLightsIntensity); //for hdrp
                emergencyStopLight.SetColor("_EmissionColor", emergencyStopColor * callerLightsIntensity); //for built-in
            }
        }

        public void EmergencyStopEnds() {
            travelSound.Stop();
            breaksSound.Stop();
            emergencyStopSound.Play();

            moving = false;
            UpdateDoorLocks();
            UpdateCallersLights();

            //red light
            emergencyStopLight.SetColor("_EmissiveColor", Color.black); //for hdrp
            emergencyStopLight.SetColor("_EmissionColor", Color.black); //for built-in
        }


        void Awake() {
            //each door will report itself on Start, so let's clean the lists.
            innerDoors.Clear();
            outterDoors.Clear();

            stopsContainer = transform.Find("Stops");
        }

        void Start() {
            originalElevatorRotation = elevator.rotation;
            elevator.position = initialStop.position;
            destination = initialStop.position;
            moving = false;
            UpdateDoorLocks(true);
            UpdateCallersLights();

            //player not provided?
            if (avoidBouncing && player == null) {
                //try to get the player by tag
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
        }

        void UpdateDoorLocks(bool silently = false) {
            foreach (DoorTrigger door in outterDoors) {
                bool shouldLock = 
                    lockOutterDoorsInOtherFloors
                    ? (Vector3.Distance(door.floorStop.position, elevator.position) > .5f)
                    : false;

                if (door.locked && !shouldLock && doorUnlockedSound != null && !silently)
                    doorUnlockedSound.Play();

                door.locked = shouldLock;

            }
            foreach (DoorTrigger door in innerDoors) {
                door.locked = false;
            }
        }


        void FixedUpdate() {
            if (moving) {
                //linear movement
                
                elevator.Translate(0, direction * currentSpeed, 0);

                if (avoidBouncing) {
                    if (player != null) {
                        if (Physics.Raycast(player.position, -Vector3.up, distToFloor, LayerMask.GetMask("ElevatorFloor"))) {
                            player.Translate(0, direction * currentSpeed, 0);
                        }
                    } else {
                        if (!hasShownPlayerWarning) {
                            hasShownPlayerWarning = true;
                            Debug.LogWarning("If you want the 'avoid bouncing' option, you should tell the Elevator Brain who the player is (Drag your player to the 'player' slot).", gameObject);
                        }
                    }
                }

                //almost there?
                if (slowDownNearArriving) {
                    if (
                       (direction == 1 && elevator.position.y > destination.y - 1f)
                       ||
                       (direction == -1 && elevator.position.y < destination.y + 1f)
                    ) {
                        currentSpeed = Mathf.Clamp( currentSpeed * (1-slowingDownEffect), .002f, 99f);
                        if (!breaking) {
                            breaking = true;
                            //travelSound.pitch = .9f;
                            travelSound.volume *= .8f;

                            breaksSound.volume = .7f;
                            breaksSound.pitch = 1f;
                            breaksSound.Play();
                        }
                    }
                }


                //has arrived?
                if (
                    (direction == 1 && elevator.position.y > destination.y)
                    ||
                    (direction == -1 && elevator.position.y < destination.y)
                ) {
                    currentShakeMagnitude = shakeWhenTravelEnds;
                    travelSound.Stop();
                    breaksSound.Stop();
                    stopSound.Play();
                    moving = false;
                    breaking = false;
                    UpdateDoorLocks();
                    UpdateCallersLights();
                }
            }

            if (currentShakeMagnitude > .01f) currentShakeMagnitude *= .9f;
        }

        bool SafetyCheck() {
            if (!moving) {
                if (innerDoorsMustBeFullyClosed) {
                    foreach (DoorTrigger door in innerDoors) {
                        if (!door.isFullyClosed) return false;
                    }
                }

                if (outterDoorsMustBeFullyClosed) {
                    foreach (DoorTrigger door in outterDoors) {
                        if (!door.isFullyClosed) return false;
                    }
                }
            }



            return true;
        }

        public void InnerDoorsChangeState() {
            if (moving) {
                //emergency stop
                if (emergencyStopIfInnerDoorsOpen) {
                    foreach (DoorTrigger door in innerDoors) {
                        if (!door.isFullyClosed) {
                            EmergencyStopBegin();
                            return;
                        }
                    }
                }
            } else {
                //resume travel after emergency stop
                if (resumeTravelAfterDoorsClose) {
                    bool fullyClosed = true;
                    foreach (DoorTrigger door in innerDoors) {
                        if (!door.isFullyClosed) fullyClosed = false;
                    }
                    if (fullyClosed && !ElevatorIsOnDestination()) {
                        GoToPosition(destination);
                    }
                }
            }
        }

        bool ElevatorIsOnDestination() {
            return (Vector3.Distance(elevator.position, destination) < .1f);
        }



        void Update() {
            if (Time.time > nextFlicker) ApplyLightsFlickering();
            if (Time.time > nextShaking) ApplyElevatorShaking();

            if (elevatorShaking != ElevatorShaking.NoShaking) {
                elevator.rotation = originalElevatorRotation * Quaternion.Euler(
                    Random.Range(-currentShakeMagnitude, currentShakeMagnitude),    
                    Random.Range(-currentShakeMagnitude, currentShakeMagnitude),    
                    Random.Range(-currentShakeMagnitude, currentShakeMagnitude)    
                );
                
            }
        }

        void ApplyLightsFlickering() {
            if (!lightsState) return; //only flicker if light is (in theory) on
            float offDuration=0, onDuration=0;
            switch (lightFlickering) {
                case FlickeringFrecuency.NoFlickering:
                    nextShaking = float.MaxValue;
                    return;

                case FlickeringFrecuency.AlmostUnnoticeable:
                    offDuration = Random.Range(.05f, .1f);
                    onDuration = Random.Range(3f, 6f);
                    break;

                case FlickeringFrecuency.Annoying:
                    offDuration = Random.Range(.08f, .2f);
                    onDuration = Random.Range(.5f, 4f);
                    break;

                case FlickeringFrecuency.Heavy:
                    offDuration = Random.Range(.08f, .2f);
                    onDuration = Random.Range(.2f, 2f);
                    break;

                case FlickeringFrecuency.Nightmare:
                    offDuration = Random.Range(.1f, 1.5f);
                    onDuration = Random.Range(.05f, .5f);
                    break;
            }
            elevatorLightsContainer.gameObject.SetActive(false);
            elevatorLightSwitchScript.UpdateTubeEmission(false);

            Invoke("LightFlickerEnds", offDuration);
            nextFlicker = Time.time + offDuration + onDuration;
            flickeringSounds[Random.Range(0, 3)].Play();
        }

        

        void ApplyElevatorShaking() {
            if (!moving) return;
            switch (elevatorShaking) {
                case ElevatorShaking.NoShaking:
                    nextShaking = float.MaxValue;
                    return;

                case ElevatorShaking.AlmostUnnoticeable:
                    currentShakeMagnitude = Random.Range(.05f, .1f) * multiplier;
                    nextShaking = Time.time + Random.Range(2f, 6f);
                    break;

                case ElevatorShaking.Annoying:
                    currentShakeMagnitude = Random.Range(.08f, .2f) * multiplier;
                    nextShaking = Time.time + Random.Range(.5f, 4f);
                    break;

                case ElevatorShaking.Heavy:
                    currentShakeMagnitude = Random.Range(.08f, .2f) * multiplier;
                    nextShaking = Time.time + Random.Range(.2f, 2f);
                    break;

                case ElevatorShaking.Nightmare:
                    currentShakeMagnitude = Random.Range(.1f, 1f) * multiplier;
                    nextShaking = Time.time + Random.Range(.5f, 1f);
                    break;
            }

        }

        void LightFlickerEnds() {
            if (lightsState) {
                elevatorLightsContainer.gameObject.SetActive(true);
                elevatorLightSwitchScript.UpdateTubeEmission(true);
                //flickeringSounds[Random.Range(0,3)].Play();
            }
        }



        void UpdateCallersLights() {
            //for hdrp:
            isMovingLight.SetColor(
                "_EmissiveColor", 
                moving ? (movingLightColor * callerLightsIntensity) : Color.black
            );
            movingUpLight.SetColor(
                "_EmissiveColor", 
                (moving && direction ==1)  ? (movingLightColor * callerLightsIntensity) : Color.black
            );
            movingDownLight.SetColor(
                "_EmissiveColor", 
                (moving && direction ==-1) ? (movingLightColor * callerLightsIntensity) : Color.black
            );

            //for built-in:
            isMovingLight.SetColor(
                "_EmissionColor",
                moving ? (movingLightColor * callerLightsIntensity) : Color.black
            );
            movingUpLight.SetColor(
                "_EmissionColor",
                (moving && direction == 1) ? (movingLightColor * callerLightsIntensity) : Color.black
            );
            movingDownLight.SetColor(
                "_EmissionColor",
                (moving && direction == -1) ? (movingLightColor * callerLightsIntensity) : Color.black
            );
        }

    }
}