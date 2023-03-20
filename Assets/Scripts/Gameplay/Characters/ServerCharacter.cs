using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Kraft.Gameplay.Characters
{
    /// <summary>
    /// Contains all NetworkVariables, RPCs and server-side logic of a character.
    /// This class was separated in two to keep client and server context self contained. This way you don't have to continuously ask yourself if code is running client or server side.
    /// </summary>
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField] ClientCharacter m_ClientCharacter;
        [SerializeField] ServerCharacterMovement m_Movement;
        [SerializeField] PhysicsWrapper m_PhysicsWrapper;
        [SerializeField] ServerAnimationHandler m_AnimationHandler;
        
        public ClientCharacter ClientCharacter => m_ClientCharacter;
        public ServerCharacterMovement Movement => m_Movement;
        public PhysicsWrapper PhysicsWrapper => m_PhysicsWrapper;
        public ServerAnimationHandler AnimationHandler => m_AnimationHandler;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }
        }

        public override void OnNetworkDespawn()
        {

        }
    }
}
