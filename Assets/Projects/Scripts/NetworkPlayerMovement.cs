using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // Disable input for remote players
            playerInput.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        var moveInput = moveAction.ReadValue<Vector2>();

        var movement = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(movement * (moveSpeed * Time.deltaTime), Space.World);
    }
}