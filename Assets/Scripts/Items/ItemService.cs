using Assets.Scripts.Items.Models;

namespace Assets.Scripts.Items
{
    public class ItemService
    {
        public ItemContainer CreateStorage(int size)
        {
            var storage = new ItemContainer(size);
            return storage;
        }
    }
}
