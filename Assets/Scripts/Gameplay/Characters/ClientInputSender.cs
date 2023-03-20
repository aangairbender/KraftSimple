using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kraft.Gameplay.Characters
{
    [RequireComponent(typeof(ServerCharacter))]
    public class ClientInputSender : NetworkBehaviour
    {
        [SerializeField] ServerCharacter m_ServerCharacter;
        [SerializeField] PhysicsWrapper m_PhysicsWrapper;

        InputActions m_InputActions;
        Vector3 m_lastDirection = Vector3.zero;

        bool m_lastSprint = false;

        public override void OnNetworkSpawn()
        {
            if (!IsClient || !IsOwner)
            {
                enabled = false;
                return;
            }

            m_InputActions = new InputActions();
            m_InputActions.Character.Enable();
        }

        private void Update()
        {
            var follow = m_ServerCharacter.ClientCharacter.CameraFollow;
            var movement = m_InputActions.Character.Movement.ReadValue<Vector2>();

            var direction = follow.right * movement.x + follow.forward * movement.y;
            direction.y = 0;

            var sprint = m_InputActions.Character.Sprint.ReadValue<float>() > 0.5f;

            if (m_lastDirection != direction)
            {
                m_lastDirection = direction;
                m_ServerCharacter.Movement.SetDesiredMoveDirectionServerRpc(direction);
            }

            if (m_lastSprint != sprint)
            {
                m_lastSprint = sprint;
                m_ServerCharacter.Movement.SetSprintingServerRpc(sprint);
            }
        }
    }
}
