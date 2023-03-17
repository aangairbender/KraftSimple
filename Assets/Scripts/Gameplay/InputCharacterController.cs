using UnityEngine;

namespace Kraft.Gameplay
{

    [RequireComponent(typeof(CharacterController))]
    public class InputCharacterController : MonoBehaviour
    {
        [Header("References")]
        public GameObject Model;
        public Transform LookFrom;
        public Transform Follow;

        [Header("Settings")]
        public float MovementSpeed;
        public float SprintSpeed;
        public float MouseSensitivity;
        public float JumpHeight;

        private InputActions inputActions;
        private CharacterController characterController;
        private Animator animator;
        private float verticalVelocity;

        private void Awake()
        {
            inputActions = new InputActions();
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
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

            var sprint = inputActions.Character.Sprint.ReadValue<float>();
            var speed = sprint * SprintSpeed + (1.0f - sprint) * MovementSpeed;

            var direction = Follow.right * movement.x + Follow.forward * movement.y;
            direction.y = 0;
            if (movement.magnitude > 0.01)
            {
                Model.transform.rotation = Quaternion.Lerp(Model.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * 15.0f);
                //Model.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            characterController.Move(direction * Time.deltaTime * speed);
            animator.SetFloat("Speed", direction.magnitude * speed);

            //var inventory = GameObject.Find("Inventory");
            //if (inventory == null)
            //{
            var lookDelta = inputActions.Character.Look.ReadValue<Vector2>();
            //transform.Rotate(Vector3.up, lookDelta.x * MouseSensitivity, Space.Self);

            //LookFrom.Rotate(Vector3.right, -lookDelta.y * MouseSensitivity, Space.Self);

            Follow.rotation *= Quaternion.AngleAxis(lookDelta.x * MouseSensitivity, Vector3.up);
            Follow.rotation *= Quaternion.AngleAxis(-lookDelta.y * MouseSensitivity, Vector3.right);

            var angles = Follow.localEulerAngles;
            angles.z = 0;

            // clamp up/down rotation
            var angle = Follow.localEulerAngles.x;
            if (angle > 180 && angle < 340)
                angles.x = 340;
            else if (angle < 180 && angle > 40)
                angles.x = 40;

            Follow.localEulerAngles = angles;
            //}

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
}
