using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kraft.SceneManagement
{
    public sealed class SceneLoaderWrapper : NetworkBehaviour
    {
        [SerializeField] LoadingScreen m_LoadingScreen;

        public static SceneLoaderWrapper Instance { get; private set; }

        private AsyncOperation m_LoadOperation;

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

        private void Update()
        {
            if (m_LoadOperation != null)
            {
                var progress = m_LoadOperation.isDone ? 1f : m_LoadOperation.progress;
                m_LoadingScreen.Progress = progress;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager?.SceneManager != null)
            {
                NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
        }

        /// <summary>
        /// Initializes the callback on scene events. This needs to be called right after initializing NetworkManager
        /// (after StartHost, StartClient or StartServer)
        /// </summary>
        public void AddOnSceneEventCallback()
        {
            if (IsNetworkSceneManagementEnabled)
            {
                NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }

        public void LoadScene(string sceneName, bool useNetworkSceneManager, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (useNetworkSceneManager)
            {
                if (IsSpawned && IsNetworkSceneManagementEnabled && !NetworkManager.ShutdownInProgress)
                {
                    if (NetworkManager.IsServer)
                    {
                        // If is active server and NetworkManager uses scene management, load scene using NetworkManager's SceneManager
                        NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
                    }
                }
            }
            else
            {
                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                if (loadSceneMode == LoadSceneMode.Single)
                {
                    m_LoadingScreen.StartLoadingScreen();
                    m_LoadOperation = loadOperation;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                m_LoadingScreen.StopLoadingScreen();
            }
        }

        void OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load: // Server told client to load a scene
                    // Only executes on client
                    if (NetworkManager.IsClient)
                    {
                        // Only start a new loading screen if scene loaded in Single mode, else simply update
                        if (sceneEvent.LoadSceneMode == LoadSceneMode.Single)
                        {
                            m_LoadingScreen.StartLoadingScreen();
                            m_LoadOperation = sceneEvent.AsyncOperation;
                        }
                        else
                        {
                            m_LoadOperation = sceneEvent.AsyncOperation;
                        }
                    }
                    break;
                case SceneEventType.LoadEventCompleted: // Server told client that all clients finished loading a scene
                    // Only executes on client
                    if (NetworkManager.IsClient)
                    {
                        m_LoadingScreen.StopLoadingScreen();
                    }
                    break;
                case SceneEventType.Synchronize: // Server told client to start synchronizing scenes
                    {
                        // todo: this is a workaround that could be removed once MTT-3363 is done
                        // Only executes on client that is not the host
                        if (NetworkManager.IsClient && !NetworkManager.IsHost)
                        {
                            // unload all currently loaded additive scenes so that if we connect to a server with the same
                            // main scene we properly load and synchronize all appropriate scenes without loading a scene
                            // that is already loaded.
                            UnloadAdditiveScenes();
                        }
                        break;
                    }
                case SceneEventType.SynchronizeComplete: // Client told server that they finished synchronizing
                    // Only executes on server
                    if (NetworkManager.IsServer)
                    {
                        // Send client RPC to make sure the client stops the loading screen after the server handles what it needs to after the client finished synchronizing, for example character spawning done server side should still be hidden by loading screen.
                        StopLoadingScreenClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { sceneEvent.ClientId } } });
                    }
                    break;
            }
        }

        void UnloadAdditiveScenes()
        {
            var activeScene = SceneManager.GetActiveScene();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != activeScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        [ClientRpc]
        void StopLoadingScreenClientRpc(ClientRpcParams clientRpcParams = default)
        {
            m_LoadingScreen.StopLoadingScreen();
        }
    }
}
