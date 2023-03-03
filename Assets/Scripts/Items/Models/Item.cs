using Assets.Scripts.Items.Data;

namespace Assets.Scripts.Items.Models
{
    public class Item
    {
        public ItemData Data { get; }

        public Item(ItemData data)
        {
            Data = data;
        }
    }
}
