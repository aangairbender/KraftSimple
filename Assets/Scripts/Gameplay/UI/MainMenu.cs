using Kraft.ConnectionManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Kraft.Gameplay
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button m_HostButton;
        [SerializeField] Button m_ClientButton;
        [SerializeField] TMP_InputField m_PlayerName;
        [SerializeField] TMP_InputField m_IpAddressInput;
        [SerializeField] TMP_InputField m_PortInput;

        [Inject] ConnectionManager m_ConnectionManager;

        public const string k_DefaultIp = "127.0.0.1";
        public const int k_DefaultPort = 9998;

        private void Awake()
        {
            m_HostButton.onClick.AddListener(() => StartHost());

            m_ClientButton.onClick.AddListener(() => StartClient());
        }

        private void StartHost()
        {
            var playerName = m_PlayerName.text;
            var ip = m_IpAddressInput.text;
            var port = m_PortInput.text;

            int.TryParse(port, out var portNum);
            if (portNum <= 0)
            {
                portNum = k_DefaultPort;
            }

            ip = string.IsNullOrEmpty(ip) ? k_DefaultIp : ip;

            m_ConnectionManager.StartHostIp(playerName, ip, portNum);
        }

        private void StartClient()
        {
            var playerName = m_PlayerName.text;
            var ip = m_IpAddressInput.text;
            var port = m_PortInput.text;

            int.TryParse(port, out var portNum);
            if (portNum <= 0)
            {
                portNum = k_DefaultPort;
            }

            ip = string.IsNullOrEmpty(ip) ? k_DefaultIp : ip;

            m_ConnectionManager.StartClientIp(playerName, ip, portNum);
        }
    }
}
