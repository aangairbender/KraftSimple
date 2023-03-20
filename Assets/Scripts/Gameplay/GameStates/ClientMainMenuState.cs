using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Kraft.Gameplay
{
    public class ClientMainMenuState : GameStateBehaviour
    {
        [SerializeField] MainMenu m_MainMenu;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(m_MainMenu);
        }
    }
}
