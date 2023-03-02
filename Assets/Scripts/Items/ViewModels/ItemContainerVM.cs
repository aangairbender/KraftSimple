using Assets.Scripts.Common;
using Assets.Scripts.Items.Models;
using UnityEngine;

namespace Assets.Scripts.Items.ViewModels
{
    public class ItemContainerVM
    {
        private readonly ItemContainer container;

        public Observable<bool> Visible { get; }

        public ItemContainer Container => container;

        public ItemContainerVM(ItemContainer container)
        {
            this.container = container;
            Visible = new Observable<bool>(false);
        }
    }
}
