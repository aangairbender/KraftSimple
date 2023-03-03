using Assets.Scripts.Common;
using Assets.Scripts.Items.Models;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Items.ViewModels
{
    public class ItemContainerVM : ViewModel
    {
        private readonly ItemContainer container;

        private bool visible;

        public bool Visible
        {
            get => visible;
            set => SetProperty(ref visible, value);
        }

        public List<Slot> Slots => Enumerable.Range(0, container.Size).Select(i => container.GetSlot(i)).ToList();

        public ItemContainer Container => container;

        public ItemContainerVM(ItemContainer container)
        {
            this.container = container;
            Visible = false;

            container.OnChanged += Container_OnChanged;
        }

        private void Container_OnChanged()
        {
            NotifyOfPropertyChange(nameof(Slots));
        }
    }
}
