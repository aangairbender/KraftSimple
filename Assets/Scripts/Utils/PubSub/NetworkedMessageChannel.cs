using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Kraft.Utils
{
    public class NetworkedMessageChannel<T> : MessageChannel<T> where T : unmanaged, INetworkSerializeByMemcpy
    {
        NetworkManager m_NetworkManager;
        readonly string m_Name;

        public NetworkedMessageChannel(NetworkManager networkManager)
        {
            m_Name = $"{typeof(T).FullName}NetworkMessageChannel";
            m_NetworkManager = networkManager;
            m_NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            if (m_NetworkManager.IsListening)
            {
                RegisterHandler();
            }
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                if (m_NetworkManager?.CustomMessagingManager != null)
                {
                    m_NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(m_Name);
                }
            }
            base.Dispose();
        }

        private void NetworkManager_OnClientConnectedCallback(ulong _)
        {
            RegisterHandler();
        }

        private void RegisterHandler()
        {
            // Only register message handler on clients
            if (!m_NetworkManager.IsServer)
            {
                m_NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(m_Name, ReceiveMessageThroughNetwork);
            }
        }

        public override void Publish(T message)
        {
            if (m_NetworkManager.IsServer)
            {
                // send message to clients, then publish locally
                SendMessageThroughNetwork(message);
                base.Publish(message);
            }
            else
            {
                Debug.LogError("Only a server can publish in a NetworkedMessageChannel");
            }
        }

        void SendMessageThroughNetwork(T message)
        {
            var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(message);
            m_NetworkManager.CustomMessagingManager.SendNamedMessageToAll(m_Name, writer);
        }

        void ReceiveMessageThroughNetwork(ulong clientId, FastBufferReader reader)
        {
            reader.ReadValueSafe(out T message);
            base.Publish(message);
        }
    }
}
