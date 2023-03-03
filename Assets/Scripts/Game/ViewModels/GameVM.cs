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
        private readonly EventLoopService eventLoopService;

        public ItemContainerVM InventoryVM { get; }

        public GameVM(EventLoopService eventLoopService)
        {
            this.eventLoopService = eventLoopService;
            eventLoopService.UpdateEvent += EventLoopService_UpdateEvent;

            inventory = new ItemContainer(16);
            InventoryVM = new ItemContainerVM(inventory);
        }

        public void Add(Item item)
        {
            inventory.Add(item, 1);
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
