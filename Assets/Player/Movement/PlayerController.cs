using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // 🔑 Solo el jugador local controla su player
        if (!photonView.IsMine)
        {
            // Desactivamos este script en players remotos
            enabled = false;
            return;
        }
    }

    void Update()
    {
        HandleMovement();

       
    }

    private void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f;

        Vector3 moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = moveDir.magnitude > 0.1f;

        if (animator != null)
        {
            animator.SetBool("isRunning", isMoving);
        }

        // Rotación hacia dirección de movimiento
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Gravedad
        if (controller.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMovement = moveDir * movementSpeed + new Vector3(0f, verticalVelocity, 0f);

        controller.Move(finalMovement * Time.deltaTime);
    }
}