using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputCharacterController : MonoBehaviour
{
    [Header("References")]
    public Transform LookFrom;

    [Header("Settings")]
    public float MovementSpeed;
    public float MouseSensitivity;
    public float JumpHeight;

    private InputActions inputActions;
    private CharacterController characterController;
    private float verticalVelocity;

    private void Awake()
    {
        inputActions = new InputActions();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        inputActions.Character.Enable();
    }

    private void OnDisable()
    {
        inputActions.Character.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void LateUpdate()
    {
        Camera.main.transform.SetPositionAndRotation(LookFrom.position, LookFrom.rotation);
    }

    private void HandleInput()
    {
        var wasGrounded = characterController.isGrounded;
        var movement = inputActions.Character.Movement.ReadValue<Vector2>();

        var direction = transform.right * movement.x + transform.forward * movement.y;
        characterController.Move(direction * Time.deltaTime * MovementSpeed);

        var lookDelta = inputActions.Character.Look.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, lookDelta.x * MouseSensitivity, Space.Self);

        LookFrom.Rotate(Vector3.right, -lookDelta.y * MouseSensitivity, Space.Self);

        if (wasGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = 0f;
            }
            var jump = inputActions.Character.Jump.ReadValue<float>();
            if (jump > 0)
            {
                verticalVelocity += JumpHeight;
            }
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
