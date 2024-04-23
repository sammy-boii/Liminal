using UnityEngine;
using System.Collections;
using UnityEngine.UIElements.Experimental;

public class ElevatorController : MonoBehaviour
{
    public GameObject elevator;
    public float moveSpeed;
    private GameObject player;

    public CameraShake cameraShake;

    public bool isMoving = false; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void MoveToFloor(Transform floor)
    {
        isMoving = true;
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
        StartCoroutine(cameraShake.Shake(.4f, .01f));

        elevator.transform.position = targetPosition;
        moveSpeed = initialMoveSpeed; 
    }
}
