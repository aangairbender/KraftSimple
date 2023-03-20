using Kraft.Utils;
using System.Collections;
using UnityEngine;
using VContainer;

namespace Kraft.ConnectionManagement
{
    public struct ReconnectMessage
    {
        public int CurrentAttempt;
        public int MaxAttempt;

        public ReconnectMessage(int currentAttempt, int maxAttempt)
        {
            CurrentAttempt = currentAttempt;
            MaxAttempt = maxAttempt;
        }
    }

    internal class ClientReconnectingState : ClientConnectingState
    {
        [Inject]
        IPublisher<ReconnectMessage> m_ReconnectMessagePublisher;

        Coroutine m_ReconnectCoroutine;
        int m_Attempts;

        // in seconds
        const float k_TimeBetweenAttempts = 5;

        public override void Enter()
        {
            m_Attempts = 0;
            m_ReconnectCoroutine = m_ConnectionManager.StartCoroutine(ReconnectCoroutine());
        }

        public override void Exit()
        {
            if (m_ReconnectCoroutine != null)
            {
                m_ConnectionManager.StopCoroutine(m_ReconnectCoroutine);
                m_ReconnectCoroutine = null;
            }
            m_ReconnectMessagePublisher.Publish(new ReconnectMessage(m_ConnectionManager.ReconnectAttempts, m_ConnectionManager.ReconnectAttempts));
        }

        public override void OnClientConnected(ulong _)
        {
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnected);
        }

        public override void OnClientDisconnected(ulong _)
        {
            var disconnectReason = m_ConnectionManager.NetworkManager.DisconnectReason;
            if (m_Attempts < m_ConnectionManager.ReconnectAttempts)
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    m_ReconnectCoroutine = m_ConnectionManager.StartCoroutine(ReconnectCoroutine());
                }
                else
                {
                    var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    m_ConnectStatusPublisher.Publish(connectStatus);
                    switch (connectStatus)
                    {
                        case ConnectStatus.UserRequestedDisconnect:
                        case ConnectStatus.HostEndedSession:
                        case ConnectStatus.ServerFull:
                        case ConnectStatus.IncompatibleBuildType:
                            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
                            break;
                        default:
                            m_ReconnectCoroutine = m_ConnectionManager.StartCoroutine(ReconnectCoroutine());
                            break;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    m_ConnectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
                }
                else
                {
                    var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    m_ConnectStatusPublisher.Publish(connectStatus);
                }

                m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
            }
        }

        IEnumerator ReconnectCoroutine()
        {
            // If not on first attempt, wait some time before trying again, so that if the issue causing the disconnect
            // is temporary, it has time to fix itself before we try again. Here we are using a simple fixed cooldown
            // but we could want to use exponential backoff instead, to wait a longer time between each failed attempt.
            // See https://en.wikipedia.org/wiki/Exponential_backoff
            if (m_Attempts > 0)
            {
                yield return new WaitForSeconds(k_TimeBetweenAttempts);
            }

            Debug.Log("Lost connection to host, trying to reconnect...");

            m_ConnectionManager.NetworkManager.Shutdown();

            yield return new WaitWhile(() => m_ConnectionManager.NetworkManager.ShutdownInProgress); // wait until NetworkManager completes shutting down
            Debug.Log($"Reconnecting attempt {m_Attempts + 1}/{m_ConnectionManager.ReconnectAttempts}...");
            m_ReconnectMessagePublisher.Publish(new ReconnectMessage(m_Attempts, m_ConnectionManager.ReconnectAttempts));
            m_Attempts++;

            var connectingClient = ConnectClientAsync();
            yield return new WaitUntil(() => connectingClient.IsCompleted);
        }
    }
}
