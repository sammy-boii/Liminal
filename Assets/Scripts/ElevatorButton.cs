using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public Transform floor;
    public ElevatorController elevator;
    public Animator anim;

    public InteriorDoor interiorDoor;

    public void Press()
    {
        if (!elevator.isMoving)
        {
            anim.SetTrigger("press");
            Invoke("MoveElevator", 0.5f);
        }
    }

    void MoveElevator()
    {
        if (interiorDoor.isOpen)
        {
            interiorDoor.Close();
        }
        elevator.MoveToFloor(floor);
    }

}
