using Assets.Scripts.Game.Services;
using Assets.Scripts.Game.ViewModels;
using Assets.Scripts.Game.Views;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameView gameView;
        [SerializeField] private EventLoopService eventLoopService;

        private void Start()
        {
            var gameVm = new GameVM(eventLoopService);
            gameView.ViewModel = gameVm;
        }
    }
}
