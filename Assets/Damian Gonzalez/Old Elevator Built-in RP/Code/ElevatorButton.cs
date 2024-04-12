using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldElevator {
    public class ElevatorButton : MonoBehaviour {
        public enum ButtonType { GoTo, LightToggle, Stop }
        public ButtonType buttonType;
        public Transform destination;
        public ElevatorBrain brain;

        [Space(10)]
        [Header("Only for light switches ____________________________________________")]
        [Space(40)]

        [SerializeField] private float angleOn = -140f;
        [SerializeField] private float angleOff = -50f;
        public LightInitialState initialState = LightInitialState.On;
        public enum LightInitialState { On, Off};
        [HideInInspector] public bool lightIsOn = true;
        public bool controlsElevatorLights = false;
        [SerializeField] private AudioSource switchOnSound, switchOffSound;
        [SerializeField] private MeshRenderer fluorescentTubeRend;
        [SerializeField] private Color fluorescentColorOn = new Color(130,130,130);
        [SerializeField] private Color fluorescentColorOff = new Color(50,50,50);
        [SerializeField] private float fluorescentIntensity = 2f;

        [Space(10)]
        [Header("Only for mechanical buttons ________________________________________")]
        [Space(40)]

        [SerializeField] private bool animateOnPress = true;
        [SerializeField] private AnimationCurve pressAnimationCurve;
        [SerializeField] private Vector3 movementFactor = new Vector3(-.2f, 0, 0);
        private float pressStartTime = 0;
        private float curveSpeed = 1f;
        private Vector3 originalLocalPosition;
        private bool isAnimating = false;


        public void Press(bool withSound = true) {
            //cool down time
            if (buttonType != ButtonType.LightToggle && Time.time < pressStartTime + 1f) return;

            pressStartTime = Time.time;

            //main action
            switch (buttonType) {
                case ButtonType.GoTo:
                    brain.GoToPosition(destination.position);
                    break;

                case ButtonType.Stop:
                    brain.EmergencyStopBegin();
                    break;

                case ButtonType.LightToggle:
                    lightIsOn = !lightIsOn;
                    transform.parent.localEulerAngles = new Vector3(
                        lightIsOn ? angleOn : angleOff,
                        90,
                        90
                    );

                    UpdateTubeEmission(lightIsOn);

                    destination.gameObject.SetActive(lightIsOn);
                    if (withSound) {
                        if (lightIsOn) switchOnSound.Play(); else switchOffSound.Play();
                    }

                    if (controlsElevatorLights) brain.ElevatorLightsToggle(lightIsOn);
                    break;
            }

            if (withSound) {
                //if there is an audio source attached, play it
                if (TryGetComponent(out AudioSource audio)) {
                    audio.pitch = Random.Range(.95f, 1.1f);
                    audio.volume = Random.Range(.8f, 1.2f);
                    audio.Play();
                }
            }

            if (animateOnPress) {
                isAnimating = true;
                curveSpeed = Random.Range(.8f, 1.2f);
            }

        }

        public void UpdateTubeEmission(bool value) {
            if (fluorescentTubeRend != null) {
                //for built-in RP
                fluorescentTubeRend.materials[2]?.SetColor(
                    "_EmissionColor",
                    value ? (fluorescentColorOn * fluorescentIntensity) : fluorescentColorOff
                );

                //for HDRP
                fluorescentTubeRend.materials[2]?.SetColor(
                    "_EmissiveColor",
                    value ? (fluorescentColorOn * fluorescentIntensity) : fluorescentColorOff
                );
            }
        }

        public void Start() {
            originalLocalPosition = transform.localPosition;

            if (buttonType == ButtonType.LightToggle) {
                lightIsOn = (initialState == LightInitialState.Off); //opposite of what it should be
                Press(false); //and change it. This includes "UpdateTubeEmission"
            }
        }


        void FixedUpdate() {
            if (animateOnPress && isAnimating) {
                float elapsedTime = (Time.time - pressStartTime) * curveSpeed;
                if (elapsedTime > 3f) isAnimating = false;

                transform.localPosition = 
                    originalLocalPosition 
                    + (movementFactor * pressAnimationCurve.Evaluate(elapsedTime))
                ; 
            }
        }

    }
}