using Assets.Scripts.Common;
using Assets.Scripts.Items.ViewModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Items.Views
{
    public class ItemContainerView : View<ItemContainerVM>
    {
        [SerializeField] private GridLayoutGroup grid;
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
            var slots = ViewModel.Slots;
            if (slotViews == null)
            {
                CreateSlots(slots.Count);
            }
            for (int i = 0; i < slots.Count; i++)
            {
                slotViews[i].Item = slots[i].Item;
                slotViews[i].Quantity = slots[i].Quantity;
            }
        }

        private void CreateSlots(int size)
        {
            slotViews = new List<SlotView>();
            for (int i = 0; i < size; i++)
            {
                var slotView = Instantiate(slotViewPrefab, grid.transform);
                slotView.name = $"Slot_{i}";
                slotView.Index = i;
                slotView.ItemContainerView = this;
                slotViews.Add(slotView);
            }
        }

        public void Drag(int fromSlotIndex, int toSlotIndex, int quantity)
        {
            ViewModel.Move(fromSlotIndex, toSlotIndex, quantity);
        }
    }
}
