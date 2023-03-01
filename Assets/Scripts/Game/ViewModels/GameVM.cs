using Assets.Scripts.Items.Models;
using Assets.Scripts.Items.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.ViewModels
{
    public class GameVM
    {
        private ItemContainer inventory;

        private ItemContainerVM inventoryVM;

        public GameVM()
        {
            inventory = new ItemContainer(16);
            inventoryVM = new ItemContainerVM(inventory);
        }
    }
}
