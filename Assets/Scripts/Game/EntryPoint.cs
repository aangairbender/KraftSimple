using Assets.Scripts.Game.Services;
using Assets.Scripts.Game.ViewModels;
using Assets.Scripts.Game.Views;
using Assets.Scripts.Items.Data;
using Assets.Scripts.Items.Models;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameView gameView;
        [SerializeField] private EventLoopService eventLoopService;

        [SerializeField] private ItemData num1Item;
        [SerializeField] private ItemData num2Item;

        private GameVM gameVm;

        private void Start()
        {
            gameVm = new GameVM(eventLoopService);
            gameView.ViewModel = gameVm;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gameVm.Add(new Item(num1Item));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gameVm.Add(new Item(num2Item));
            }
        }
    }
}
