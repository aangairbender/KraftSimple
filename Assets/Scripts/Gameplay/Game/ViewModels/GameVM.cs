using Assets.Scripts.Common;
using Assets.Scripts.Items.Models;
using Assets.Scripts.Items.ViewModels;

namespace Assets.Scripts.Game.ViewModels
{
    public class GameVM : ViewModel
    {
        private ItemContainer inventory;
        private ItemContainer hotbar;

        public ItemContainerVM InventoryVM { get; }
        public ItemContainerVM HotbarVM { get; }

        public GameVM()
        {
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

        public void ToggleInventory()
        {
            InventoryVM.Visible = !InventoryVM.Visible;
        }
    }
}
