using UnityEngine;
using UnityEngine.AI;

public class PlayerFollow : MonoBehaviour
{
    public float catchDistance = 1f;

    [Space(10)]
    [Header("Players ____________________________________________________________")]
    [Space(10)]

    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;

    [Space(10)]
    [Header("Cameras ____________________________________________________________")]
    [Space(10)]

    public Camera playerCam;
    public Camera jumpscareCam;

    [Space(10)]
    [Header("UI Settings ____________________________________________________________")]
    [Space(10)]

    public GameObject keypadUI;
    public GameObject gameOverUI;

    private GameObject jumpscareRoom;
    private bool isUI = false;

    private void Awake()
    {
        jumpscareRoom = GameObject.Find("Animated JumpScare Container");
        jumpscareRoom.SetActive(false);
    }

    private void Start()
    {
        if (jumpscareCam)
        {
            jumpscareCam.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= catchDistance)
        {
            jumpscareRoom.SetActive(true);
            keypadUI.SetActive(false);
            playerCam.gameObject.SetActive(false);
            jumpscareCam.gameObject.SetActive(true);
            if (!isUI)
            {
                Invoke("GameOverUI", 3f);
                isUI = true;
            }
        }
        else
        {
            anim.SetTrigger("chase");
            agent.destination = player.position;
        }
    }

    void GameOverUI()
    {
        player.GetComponent<PlayerController>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverUI.SetActive(true);
    }
}
