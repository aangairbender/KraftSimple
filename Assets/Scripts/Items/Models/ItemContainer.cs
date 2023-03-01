using System;
using System.Linq;

namespace Assets.Scripts.Items.Models
{
    public class ItemContainer
    {
        private readonly Slot[] slots;

        public event Action<int> OnSlotChanged;

        public int Size => slots.Length;

        public ItemContainer(int size)
        {
            slots = new Slot[size];
        }

        public Slot GetSlot(int index)
        {
            if (index < 0 || index >= slots.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            return slots[index];
        }

        /// <summary>
        /// Adds item(s) to the container
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="quantity">Quantity of item to add</param>
        /// <returns>Quantity added to the container (might be less than desired)</returns>
        public int Add(Item item, int quantity)
        {
            var desiredQuantity = quantity;
            // filling existing slots with target item
            for (int i = 0; i < slots.Length; i++)
            {
                ref Slot slot = ref slots[i];
                if (slot.Item != item) continue;

                int spaceLeft = item.Data.StackLimit - slot.Quantity;
                int taken = Math.Min(quantity, spaceLeft);

                slot.Quantity += taken;
                quantity -= taken;
            }

            // filling empty slots with target item
            for (int i = 0; i < slots.Length; i++)
            {
                ref Slot slot = ref slots[i];
                if (slot.Item != null) continue;

                int taken = Math.Min(quantity, item.Data.StackLimit);
                if (taken > 0)
                {
                    slot.Item = item;
                    slot.Quantity = taken;
                    quantity -= taken;
                }
            }

            return desiredQuantity - quantity;
        } 

        public bool Remove(Item item, int quantity)
        {
            var availableQuantity = slots.Where(s => s.Item == item).Sum(s => s.Quantity);
            if (availableQuantity < quantity) return false;

            // iterating in backwards direction
            for (int i = slots.Length - 1; i >= 0; i--)
            {
                ref Slot slot = ref slots[i];
                if (slot.Item != item) continue;

                int taken = Math.Min(quantity, slot.Quantity);
                slot.Quantity -= taken;
                quantity -= taken;
            }

            return true;
        }

        public void Move(int sourceSlotIndex, int destinationSlotIndex)
        {
            if (sourceSlotIndex < 0 || sourceSlotIndex >= slots.Length)
                throw new ArgumentOutOfRangeException(nameof(sourceSlotIndex));
            if (destinationSlotIndex < 0 || destinationSlotIndex >= slots.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationSlotIndex));

            ref Slot source = ref slots[sourceSlotIndex];
            ref Slot destination = ref slots[destinationSlotIndex];

            if (source.Item == null) return;

            if (destination.Item == null)
            {
                destination.Item = source.Item;
                destination.Quantity = source.Quantity;
                source.Item = null;
                source.Quantity = 0;
            }
            else if (destination.Item == source.Item)
            {
                int canMove = destination.Item.Data.StackLimit - destination.Quantity;
                int moved = Math.Min(canMove, source.Quantity);
                source.Quantity -= moved;
                destination.Quantity += moved;
            }
        }
    }
}
