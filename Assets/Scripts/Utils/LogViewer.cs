using UnityEngine;
using TMPro;

namespace Kraft.Utils
{
    public class LogViewer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_TextMeshPro;

        private void Awake()
        {
            Application.logMessageReceived += Application_logMessageReceived;
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= Application_logMessageReceived;
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            m_TextMeshPro.text += $"[{type}] {condition}\n";
            if (type == LogType.Error)
            {
                m_TextMeshPro.text += $"{stackTrace}\n";
            }
        }
    }
}
