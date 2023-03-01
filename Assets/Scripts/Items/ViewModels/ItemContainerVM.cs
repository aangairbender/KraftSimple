using Assets.Scripts.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items.ViewModels
{
    public class ItemContainerVM
    {
        private readonly ItemContainer container;

        public ItemContainer Container => container;

        public ItemContainerVM(ItemContainer container)
        {
            this.container = container;
        }
    }
}
