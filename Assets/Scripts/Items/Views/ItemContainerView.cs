using Assets.Scripts.Items.ViewModels;
using UnityEngine;

namespace Assets.Scripts.Items.Views
{
    public class ItemContainerView : MonoBehaviour
    {
        [SerializeField] private ItemContainerVM vm;
        [SerializeField] private SlotView slotPrefab;

        public void Start()
        {
            for (int i = 0; i < vm.Container.Size; ++i)
            {
                var slot = vm.Container.GetSlot(i);
                var view = Instantiate(slotPrefab);
                view.transform.SetParent(this.transform, false);
                view.SetSlot(slot);
            }
        }
    }
}
