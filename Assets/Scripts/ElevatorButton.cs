using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public Transform floor;
    public ElevatorController elevator;
    public Animator anim;

    private AudioSource click;

    public InteriorDoor interiorDoor;

    public void Start()
    {
        click = GetComponent<AudioSource>();
    }

    public void Press()
    {
        if (!elevator.isMoving)
        {
            click.Play();
            anim.SetTrigger("press");
            interiorDoor.Interact();
            Invoke("MoveElevator", 1f);
        }
    }

    void MoveElevator()
    {
        elevator.MoveToFloor(floor);
    }

}
