using System;
using UnityEngine;

namespace Assets.Scripts.Game.Services
{
    public class EventLoopService : MonoBehaviour
    {
        public event Action UpdateEvent;

        private void Update()
        {
            UpdateEvent?.Invoke();
        }

        private void OnDestroy()
        {
            UpdateEvent = delegate { };
        }
    }
}
