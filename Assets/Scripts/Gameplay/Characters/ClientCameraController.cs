using Unity.Netcode;
using UnityEngine;

namespace Kraft.Gameplay.Characters
{
    public class ClientCameraController : NetworkBehaviour
    {
        [SerializeField] Transform m_CameraFollow;
        [SerializeField] float m_MouseSensitivity;

        InputActions m_InputActions;

        private void Awake()
        {
            enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                return;
            }

            enabled = true;
            m_InputActions = new InputActions();
            m_InputActions.Character.Enable();
        }

        private void Update()
        {
            var lookDelta = m_InputActions.Character.Look.ReadValue<Vector2>();

            m_CameraFollow.rotation *= Quaternion.AngleAxis(lookDelta.x * m_MouseSensitivity, Vector3.up);
            m_CameraFollow.rotation *= Quaternion.AngleAxis(-lookDelta.y * m_MouseSensitivity, Vector3.right);

            // clamp up/down rotation
            var angles = m_CameraFollow.localEulerAngles;
            angles.z = 0;

            var angle = m_CameraFollow.localEulerAngles.x;
            if (angle > 180 && angle < 340)
                angles.x = 340;
            else if (angle < 180 && angle > 40)
                angles.x = 40;

            m_CameraFollow.localEulerAngles = angles;
        }
    }
}
