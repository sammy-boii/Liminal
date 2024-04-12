#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OldElevator {
    public class ElevatorBuilder : MonoBehaviour {
        [Space(10)]
        [Header("New Elevator Setup _________________________________________")]
        [Space(40)]
        [SerializeField] [Range(2, 30)] private int floors = 5;
        [SerializeField] private int firstNumber = 1;
        [SerializeField] private float distanceBetweenFloors = 2;
        [SerializeField] private bool addFloorNumbersInPitWalls = true;
        [SerializeField] private bool interactWithInnerDoorsPanesAsOne = true;
        [SerializeField] private Transform player;
        [SerializeField] private bool addDustParticlesInElevator = true;

        [Space(10)]
        [Header("Halls Setup _______________________________________________")]
        [Space(40)]
        [SerializeField] private bool addHalls = true;
        [SerializeField] private bool addFloorNumbersInHalls = true;
        public enum CallerType { Stylized, Squared }
        [SerializeField] private CallerType callerType = CallerType.Stylized;
        [SerializeField] private bool addOuterDoors = true;
        [SerializeField] private DoorTrigger.DoorType outterDoorsType = DoorTrigger.DoorType.Hinged;
        [SerializeField] private bool interactWithOuterDoorsPanesAsOne = true;
        [SerializeField] private bool addDustParticlesInHalls = true;


        [Space(10)]
        [Header("Buttons Pad Setup _________________________________________")]
        [Space(40)]
        [SerializeField] private ButtonsLayout buttonsLayout = ButtonsLayout.Auto;
        enum ButtonsLayout { SingleColumn, TwoColumns, ThreeColumns, Auto}
        [SerializeField] private float verticalSpaceBetweenButtons = 0.04f;



        [Space(10)]
        [Header("Parts Prefabs ______________________________________________")]
        [Space(40)]
        [SerializeField] private GameObject elevatorBrain;
        [SerializeField] private GameObject mainElevator;
        [SerializeField] private GameObject hall;
        [SerializeField] private GameObject entranceCase;
        [SerializeField] private GameObject innerCase;
        [SerializeField] private GameObject topCase;
        [SerializeField] private GameObject bottomCase;
        [SerializeField] private GameObject buttonAndNumber;
        [SerializeField] private GameObject callerA;
        [SerializeField] private GameObject callerB;
        [SerializeField] private GameObject externalDoors;
        [SerializeField] private Sprite[] numberSprites = new Sprite[10];


        [ContextMenu ("Build")]
        public void Build() {

            Debug.Log("Building...");

            //__________________________ PART 1: GENERAL SETUP __________________________

            //main container
            Transform container = new GameObject("My old elevator").transform;
            container.parent = transform;
            container.localPosition = Vector3.zero;
            container.localRotation = Quaternion.identity;
            container.localScale = new Vector3(1, 1, 1);

            //elevator itself
            float y = 0;
            GameObject elevator = InstPrefab(mainElevator, y, 0, container);

            //elevator brain
            Transform brainTransform = InstPrefab(elevatorBrain, y, 0, container).transform;

            ElevatorBrain brain = brainTransform.GetComponent<ElevatorBrain>();
            brain.elevator = elevator.transform;
            brain.player = player;

            //set the inner doors up
            foreach (DoorTrigger door in elevator.GetComponentsInChildren<DoorTrigger>()) {
                door.doorLocation = DoorTrigger.DoorLocation.Internal;
                door.brain = brain;
                door.floorStop = null;
                door.locked = false;
                door.interactWithBoth = interactWithInnerDoorsPanesAsOne;
            }

            //stops container
            Transform stopsContainer = new GameObject("Stops").transform;
            stopsContainer.parent = brainTransform;
            stopsContainer.localPosition = Vector3.zero;

            //ref. to elevators lights
            brain.elevatorLightsContainer = elevator.transform.Find("Lights").gameObject;
            brain.elevatorLightSwitchScript = 
                elevator.transform
                .Find("Light Switch")
                .Find("Light Button Pivot Point")
                .Find("Light Button")
                .GetComponent<ElevatorButton>();

            //ref. to audio sources
            Transform soundsContainer = elevator.transform.Find("Sounds");
            brain.travelSound = soundsContainer.GetChild(0)?.GetComponent<AudioSource>();
            brain.stopSound   = soundsContainer.GetChild(1)?.GetComponent<AudioSource>();
            brain.emergencyStopSound = soundsContainer.GetChild(2)?.GetComponent<AudioSource>();

            brain.flickeringSounds[0] = soundsContainer.GetChild(3)?.GetComponent<AudioSource>();
            brain.flickeringSounds[1] = soundsContainer.GetChild(4)?.GetComponent<AudioSource>();
            brain.flickeringSounds[2] = soundsContainer.GetChild(5)?.GetComponent<AudioSource>();

            brain.doorUnlockedSound = soundsContainer.GetChild(6)?.GetComponent<AudioSource>();
            brain.breaksSound = soundsContainer.GetChild(7)?.GetComponent<AudioSource>();

            //dust in elevator?
            elevator.transform.Find("Dust particles")?.gameObject.SetActive(addDustParticlesInElevator);


            //building
            Transform building = new GameObject("Building").transform;
            building.parent = container;
            building.localPosition = Vector3.zero;

            //elevator pit (cases container)
            Transform pit = new GameObject("Elevator pit").transform;
            pit.parent = building;
            pit.localPosition = Vector3.zero;

            //bottom case
            InstPrefab(bottomCase, y - 1.5f, -90, pit, 140);

            //buttons pad
            int columns = 1;
            switch(buttonsLayout) {
                case ButtonsLayout.SingleColumn:    columns = 1; break;
                case ButtonsLayout.TwoColumns:      columns = 2; break;
                case ButtonsLayout.ThreeColumns:    columns = 3; break;
                case ButtonsLayout.Auto:
                    columns = 1;
                    if (floors > 9) columns = 2;
                    if (floors > 20) columns = 3;
                    break;
            }


            Transform pad = new GameObject("Buttons pad").transform;
            pad.parent = elevator.transform;
            float padZPos = 0, yButtons = 0, xButtons = 0;

            float horizontalSpaceBetweenButtons = 0.08f;
            yButtons = -(floors / (float)columns) * verticalSpaceBetweenButtons / 2f ;

            switch (columns) {
                case 1: padZPos = 0.632f; break;
                case 2: padZPos = 0.59f;  yButtons -= verticalSpaceBetweenButtons; break;
                case 3: padZPos = 0.56f;  horizontalSpaceBetweenButtons = 0.065f; break;
            }

            pad.transform.localPosition = new Vector3(-1.2422f, 1.154f, padZPos);
            pad.transform.localRotation = Quaternion.Euler(0, -90, 0);







            //__________________________ PART 2: EACH FLOOR ____________________________
            Transform thisHall;
            for (int i = 0; i < floors; i++) {
                int floorNumber = i + firstNumber;
                //save this position
                Transform thisStop = new GameObject("Elevator Stop - Floor " + floorNumber.ToString()).transform;
                thisStop.parent = stopsContainer;
                thisStop.localPosition = new Vector3(0, y, 0);

                if (i == 0) brain.initialStop = thisStop;

                //hall
                thisHall = InstPrefab(hall, y, 0, building).transform;
                thisHall.name = "Hall - Floor" + floorNumber.ToString();
                Transform functionalHall = thisHall.GetChild(0);
                Transform nonFunctionalHall = thisHall.GetChild(1);

                //door types
                Transform extHingedDoors = functionalHall.GetChild(1);
                Transform extAccordionDoors = functionalHall.GetChild(2);

                extHingedDoors.gameObject.SetActive(
                    addOuterDoors && outterDoorsType == DoorTrigger.DoorType.Hinged
                );
                extAccordionDoors.gameObject.SetActive(
                    addOuterDoors && outterDoorsType == DoorTrigger.DoorType.Accordion
                );

                if (addHalls) {
                    //and set (or hide) the floor numbers on the hall walls
                    SpriteRenderer[] everySR = thisHall.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer sr in everySR) {
                        if (sr.gameObject.name.Contains("Big Number")) {
                            if (addFloorNumbersInHalls) {
                                NumberInSprite(sr, floorNumber);
                            } else {
                                sr.gameObject.SetActive(false);
                            }
                        }
                    }


                    if (addOuterDoors) {
                        //set the external doors up
                        foreach (DoorTrigger door in thisHall.GetComponentsInChildren<DoorTrigger>()) {
                            door.doorLocation = DoorTrigger.DoorLocation.External;
                            door.brain = brain;
                            door.floorStop = thisStop;
                            door.interactWithBoth = interactWithOuterDoorsPanesAsOne;
                        }
                    }

                    //dust in halls?
                    nonFunctionalHall.Find("Dust particles")?.gameObject.SetActive(addDustParticlesInHalls);

                } else {
                    //remove the non-functional part of the hall
                    nonFunctionalHall.gameObject.SetActive(false);
                }

                //caller
                InstPrefab(
                    (callerType == CallerType.Stylized) ? callerA : callerB,
                    (callerType == CallerType.Stylized) ? Vector3.zero : new Vector3(-0.201f, 0.082f, 0.594f), 
                    Quaternion.identity,
                    thisHall.GetChild(0)
                );

                

                //entrance case
                InstPrefab(entranceCase, y + 1.5f, -90, pit);


                //if it's not the last hall, add a middle case above
                if (i != floors-1) {
                    Transform thisCase = InstPrefab(
                        innerCase, 
                        y + 3 + (distanceBetweenFloors/2),
                        -90, 
                        pit,
                        140
                    ).transform;

                    thisCase.localScale = new Vector3(
                        thisCase.localScale.x,
                        thisCase.localScale.y,
                        thisCase.localScale.z / 3f * distanceBetweenFloors
                    );

                    //and set (or hide) the floor numbers on the pit walls
                    foreach (SpriteRenderer sr in thisCase.GetComponentsInChildren<SpriteRenderer>()) {
                        if (addFloorNumbersInPitWalls) {

                            if (sr.gameObject.name.Contains("Upper")) {
                                NumberInSprite(sr, floorNumber + 1);
                            }
                            if (sr.gameObject.name.Contains("Lower")) {
                                NumberInSprite(sr, floorNumber);
                            }
                        } else {
                            sr.gameObject.SetActive(false);
                        }
                    }

                    y += 3 + distanceBetweenFloors;
                }


                //add a button in the pad
                Vector3 thisBtnAndNumPos = Vector3.zero;
                if (columns == 1) {
                    thisBtnAndNumPos = new Vector3(xButtons, yButtons, 0);
                    yButtons += verticalSpaceBetweenButtons;
                } else {
                    if (i % columns == 0) {
                        //first button in row
                        yButtons += verticalSpaceBetweenButtons;
                        thisBtnAndNumPos = new Vector3(xButtons, yButtons, 0);
                    } else {
                        //other
                        thisBtnAndNumPos = new Vector3(
                            xButtons + horizontalSpaceBetweenButtons * (i % columns),
                            yButtons,
                            0
                        );
                        
                    }
                }

                GameObject thisBtnAndNum = InstPrefab(
                    buttonAndNumber,
                    thisBtnAndNumPos,
                    Quaternion.identity,
                    pad
                );

                //tell the pad button where to send the elevator
                ElevatorButton thisButton = thisBtnAndNum.transform.GetChild(0).GetComponent<ElevatorButton>();
                thisButton.destination = thisStop;
                thisButton.brain = brain;

                //change the button label
                NumberInSprite(
                    thisBtnAndNum.transform.GetChild(1).GetComponent<SpriteRenderer>(),
                    floorNumber
                );


                //and also tell both buttons of the caller in this floor where to send the elevator
                foreach (ElevatorButton btn in thisHall.GetComponentsInChildren<ElevatorButton>()) { 
                    if (btn.buttonType == ElevatorButton.ButtonType.GoTo) btn.destination = thisStop;
                    btn.brain = brain;
                }


            }


            //__________________________ PART 3: WRAPPING UP __________________________

            //finally, the top case
            InstPrefab(topCase, y + 3f + 1.5f, -90, pit, 140);

            //just in case, let's tell every door and button to obbey to this elevator
            foreach (DoorTrigger door in transform.GetComponentsInChildren<DoorTrigger>()) {
                door.brain = brain;
            }
            foreach (ElevatorButton btn in transform.GetComponentsInChildren<ElevatorButton>()) {
                btn.brain = brain;
            }

            Debug.Log("Building finished successfully!");
        }

        [ContextMenu("Clear all")]
        public void ClearAll() {
            Debug.Log("Clearing...");
            for (int i = transform.childCount-1; i >-1 ; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            Debug.Log("All cleared!");
        }

        [ContextMenu("Detach")]
        public void Detach() {
            if (transform.childCount > 0) {
                Transform elevator = transform.GetChild(0);
                elevator.parent = null;
                Debug.Log("'" + elevator.name + "' successfully detached", elevator);
            } else {
                Debug.LogWarning("You have to build first in order to detach the elevator", gameObject);
            }

        }

        GameObject InstPrefab(GameObject original, float yPos= 0, float xRot=0, float scale=1) {
            return InstPrefab(original, yPos, xRot, transform, scale);
        }

        GameObject InstPrefab(GameObject original, float yPos, float xRot, Transform parent, float scale = 1) {
            return InstPrefab(original, new Vector3 (0,yPos,0), Quaternion.Euler(xRot, 0, 0), parent, scale);
        }

        GameObject InstPrefab(GameObject original, Vector3 pos, Quaternion rot, Transform parent, float scale = 1) {
            GameObject go = PrefabUtility.InstantiatePrefab(original, parent) as GameObject;
            go.transform.localPosition = pos;
            go.transform.localRotation = rot;
            go.transform.localScale = new Vector3(scale, scale, scale);
            return go;
        }

        void NumberInSprite(SpriteRenderer sr, int num) {
            sr.sprite = numberSprites[num % 10];
            if (num > 9) {
                //move the original sprite a little to the right
                Transform orig = sr.gameObject.transform;
                orig.position += orig.right * (orig.localScale.x / 2f);

                //make a copy of the same object
                Transform firstDigit = Instantiate(
                    sr.gameObject,
                    orig.position - orig.right * orig.localScale.x * 1.4f,
                    orig.rotation,
                    orig.parent
                ).transform;

                firstDigit.name = orig.name + " - " + num.ToString().Substring(0, 1);
                firstDigit.GetComponent<SpriteRenderer>().sprite = numberSprites[
                    int.Parse(num.ToString().Substring(0, 1))
                ];

            }
            sr.gameObject.name += " - " + (num % 10).ToString();
        }
    }
}
#endif