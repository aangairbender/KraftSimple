using Assets.Scripts.Common;
using Assets.Scripts.Game.Services;
using Assets.Scripts.Items.Models;
using Assets.Scripts.Items.ViewModels;
using UnityEngine;

namespace Assets.Scripts.Game.ViewModels
{
    public class GameVM : ViewModel
    {
        private ItemContainer inventory;
        private ItemContainer hotbar;

        public ItemContainerVM InventoryVM { get; }
        public ItemContainerVM HotbarVM { get; }

        public GameVM(EventLoopService eventLoopService)
        {
            eventLoopService.UpdateEvent += EventLoopService_UpdateEvent;

            inventory = new ItemContainer(48);
            InventoryVM = new ItemContainerVM(inventory);

            hotbar = new ItemContainer(10);
            HotbarVM = new ItemContainerVM(hotbar)
            {
                Visible = true
            };
        }

        public void Add(Item item)
        {
            inventory.Add(item, 1);
            hotbar.Add(item, 1);
        }

        private void EventLoopService_UpdateEvent()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                InventoryVM.Visible = !InventoryVM.Visible;
            }
        }
    }
}
