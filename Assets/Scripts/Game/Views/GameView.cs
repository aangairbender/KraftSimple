using Assets.Scripts.Common;
using Assets.Scripts.Game.ViewModels;
using Assets.Scripts.Items.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Views
{
    public class GameView : View<GameVM>
    {
        [SerializeField] private ItemContainerView inventory;

        protected override void OnViewModelSet()
        {
            inventory.ViewModel = ViewModel.InventoryVM;
        }

        protected override void OnViewModelUnset()
        {
            inventory.ViewModel = null;
        }
    }
}
