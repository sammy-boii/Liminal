using UnityEngine;
using System.Collections;
using UnityEngine.UIElements.Experimental;

public class ElevatorController : MonoBehaviour
{
    public GameObject elevator;
    public float moveSpeed;
    private GameObject player;

    public CameraShake cameraShake;

    public InteriorDoor interiorDoor;

    public bool isMoving = false;

    public AudioSource stopClip;
    public AudioSource moveClip;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void MoveToFloor(Transform floor)
    {
        if (elevator.transform.position == floor.position)
        {
            return;
        }

        interiorDoor.Close();

        isMoving = true;
        moveClip.Play();
        StartCoroutine(MoveElevatorCoroutine(floor.position));
    }

    private IEnumerator MoveElevatorCoroutine(Vector3 targetPosition)
    {
        float initialMoveSpeed = moveSpeed;

        while (Vector3.Distance(elevator.transform.position, targetPosition) > 0.1f)
        {
            float distanceRemaining = Vector3.Distance(elevator.transform.position, targetPosition);

            float speedFactor = Mathf.Clamp01(distanceRemaining / 2f);

            moveSpeed = initialMoveSpeed * speedFactor;

            Vector3 direction = (targetPosition - elevator.transform.position).normalized;
            elevator.transform.Translate(direction * moveSpeed * Time.deltaTime);

            if (player != null)
            {
                player.transform.position += direction * moveSpeed * Time.deltaTime;
            }
            yield return null;
        }

        isMoving = false;
        moveClip.Stop();
        stopClip.Play();
        StartCoroutine(cameraShake.Shake(.45f, .008f));

        Invoke("OpenDoor", 0.5f);

        elevator.transform.position = targetPosition;
        moveSpeed = initialMoveSpeed; 
    }

    void OpenDoor()
    {
        interiorDoor.Open();
    }
}
