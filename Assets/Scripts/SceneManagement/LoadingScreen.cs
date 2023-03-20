using System.Collections;
using UnityEngine;

namespace Kraft.SceneManagement
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_CanvasGroup;
        [SerializeField] float m_DelayBeforeFadeOut = 0.5f;
        [SerializeField] float m_FadeOutDuration = 0.1f;
        [SerializeField] RectTransform m_Filling;

        private bool m_LoadingScreenRunning;
        private Coroutine m_FadeOutCoroutine;
        private float m_Progress;

        public float Progress
        {
            get => m_Progress;
            set
            {
                if (m_Progress == value) return;
                m_Progress = value;

                var scale = m_Filling.localScale;
                scale.x = m_Progress;
                m_Filling.localScale = scale;
            }
        }

        public void StartLoadingScreen()
        {
            Progress = 0f;
            SetCanvasVisibility(true);
            m_LoadingScreenRunning = true;
        }

        public void StopLoadingScreen()
        {
            if (!m_LoadingScreenRunning) return;

            if (m_FadeOutCoroutine != null)
            {
                StopCoroutine(m_FadeOutCoroutine);
            }
            m_FadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            Progress = 0f;
            SetCanvasVisibility(false);
        }

        private void SetCanvasVisibility(bool visible)
        {
            m_CanvasGroup.alpha = visible ? 1 : 0;
            m_CanvasGroup.blocksRaycasts = visible;
        }

        IEnumerator FadeOutCoroutine()
        {
            yield return new WaitForSeconds(m_DelayBeforeFadeOut);
            m_LoadingScreenRunning = false;

            float currentTime = 0f;
            while (currentTime < m_FadeOutDuration)
            {
                m_CanvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / m_FadeOutDuration);
                yield return null;
                currentTime += Time.deltaTime;
            }

            SetCanvasVisibility(false);
        }
    }
}
