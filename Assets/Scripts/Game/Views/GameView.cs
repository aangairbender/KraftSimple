using Assets.Scripts.Common;
using Assets.Scripts.Game.ViewModels;
using Assets.Scripts.Items.Views;
using UnityEngine;

namespace Assets.Scripts.Game.Views
{
    public class GameView : View<GameVM>
    {
        [SerializeField] private ItemContainerView inventory;

        protected override void SetupViewModelBindings()
        {
            Bind(vm => vm.InventoryVM, () => inventory.ViewModel = ViewModel.InventoryVM);
        }
    }
}
