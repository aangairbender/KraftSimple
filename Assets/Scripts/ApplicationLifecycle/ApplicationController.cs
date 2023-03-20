using Kraft.ConnectionManagement;
using Kraft.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Kraft.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        [SerializeField] ConnectionManager m_ConnectionManager;
        [SerializeField] NetworkManager m_NetworkManager;

        DisposableGroup m_Subscriptions = new DisposableGroup();

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponent(m_ConnectionManager);
            builder.RegisterComponent(m_NetworkManager);

            // these message channels are essential and persist for the lifetime of the application
            builder.RegisterInstance(new MessageChannel<QuitApplicationMessage>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<ConnectStatus>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<ReconnectMessage>()).AsImplementedInterfaces();

            // these are networked so that the clients can subscribe to those messages that are published by the server
            builder.Register<NetworkedMessageChannel<ConnectionEvent>>(Lifetime.Singleton).AsImplementedInterfaces();
            //builder.RegisterInstance(new NetworkedMessageChannel<ConnectionEvent>()).AsImplementedInterfaces();
        }

        private void Start()
        {
            var quitApplicationSub = Container.Resolve<ISubscriber<QuitApplicationMessage>>();
            m_Subscriptions.Add(quitApplicationSub.Subscribe(QuitGame));

            var connectStatusSub = Container.Resolve<ISubscriber<ConnectStatus>>();
            m_Subscriptions.Add(connectStatusSub.Subscribe(status => Debug.Log(status)));

            Application.wantsToQuit += Application_wantsToQuit;
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        protected override void OnDestroy()
        {
            m_Subscriptions.Dispose();
            base.OnDestroy();
        }

        private bool Application_wantsToQuit()
        {
            return true;
        }

        private void QuitGame(QuitApplicationMessage _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
