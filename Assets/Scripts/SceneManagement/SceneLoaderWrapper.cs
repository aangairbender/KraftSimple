using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kraft.SceneManagement
{
    public sealed class SceneLoaderWrapper : NetworkBehaviour
    {
        public static SceneLoaderWrapper Instance { get; private set; }

        private bool IsNetworkSceneManagementEnabled => NetworkManager?.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"Multiple instances of {nameof(SceneLoaderWrapper)}");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            base.OnDestroy();
        }

        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (loadSceneMode== LoadSceneMode.Single)
            {
                // TODO: show loading screen
                // TODO: use load operation to track progress
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            // TODO: should hide loading screen
        }
    }
}
