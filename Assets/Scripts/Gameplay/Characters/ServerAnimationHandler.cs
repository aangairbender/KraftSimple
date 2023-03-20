using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Kraft.Gameplay.Characters
{
    public class ServerAnimationHandler : NetworkBehaviour
    {
        [SerializeField] NetworkAnimator m_NetworkAnimator;

        public NetworkAnimator NetworkAnimator => m_NetworkAnimator;

        public void SetSpeed(float speed)
        {
            NetworkAnimator.Animator.SetFloat("Speed", speed);
        }
    }
}
