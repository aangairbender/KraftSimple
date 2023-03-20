using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kraft.Utils
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera m_VirtualCamera;

        public void AttachVirtualCamera(Transform follow)
        {
            m_VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            Assert.IsNotNull(m_VirtualCamera, "CameraController.AttachCamera: Couldn't find gameplay virtual camera");

            if (m_VirtualCamera)
            {
                // camera body / aim
                m_VirtualCamera.Follow = follow;
            }
        }
    }
}
