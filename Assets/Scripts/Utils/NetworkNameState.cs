using Unity.Netcode;
using UnityEngine;

namespace Kraft.Utils
{
    public class NetworkNameState : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkVariable<FixedPlayerName> Name = new NetworkVariable<FixedPlayerName>();
    }
}
