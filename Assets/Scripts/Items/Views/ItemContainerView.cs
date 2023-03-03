using Assets.Scripts.Common;
using Assets.Scripts.Items.ViewModels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Items.Views
{
    public class ItemContainerView : View<ItemContainerVM>
    {
        [SerializeField] private SlotView slotViewPrefab;

        private List<SlotView> slotViews;

        protected override void SetupViewModelBindings()
        {
            Bind(vm => vm.Visible, Visible_OnChanged);
            Bind(vm => vm.Slots, Slots_OnChanged);
        }

        private void Visible_OnChanged()
        {
            gameObject.SetActive(ViewModel.Visible);
        }

        private void Slots_OnChanged()
        {
            if (slotViews == null)
            {
                CreateSlots();
            }
            var slots = ViewModel.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                slotViews[i].Item = slots[i].Item;
                slotViews[i].Quantity = slots[i].Quantity;
            }
        }

        private void CreateSlots()
        {
            var slots = ViewModel.Slots;
            slotViews = new List<SlotView>();
            for (int i = 0; i < slots.Count; i++)
            {
                var slotView = Instantiate(slotViewPrefab, gameObject.transform);
                slotView.name = $"Slot_{i}";
                slotViews.Add(slotView);
            }
        }
    }
}
