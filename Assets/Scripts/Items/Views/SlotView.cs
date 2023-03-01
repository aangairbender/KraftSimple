using Assets.Scripts.Items.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Items.Views
{
    public class SlotView : MonoBehaviour,
        IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        [SerializeField] private Image item;
        [SerializeField] private GameObject quantityRoot;
        [SerializeField] private TextMeshProUGUI quantityCaption;

        private Slot slot;
        private GameObject dragPhantom;

        public void SetSlot(Slot slot)
        {
            UnsubscribeFromSlot();
            this.slot = slot;
            SubscribeToSlot();
            PresentSlot();
        }

        private void OnDestroy()
        {
            UnsubscribeFromSlot();
        }

        private void SubscribeToSlot()
        {
            //slot.OnChanged += Slot_OnChanged;
        }

        private void UnsubscribeFromSlot()
        {
            //if (slot == null) return;

            //slot.OnChanged -= Slot_OnChanged;
        }

        private void Slot_OnChanged()
        {
            PresentSlot();
        }

        private void PresentSlot()
        {
            //item.sprite = slot.Item?.Sprite;
            quantityCaption.text = slot.Quantity.ToString();
            quantityRoot.SetActive(slot.Quantity > 1);

            item.gameObject.SetActive(slot.Item != null);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragPhantom = Instantiate(this.gameObject, Input.mousePosition, Quaternion.identity);
            dragPhantom.transform.SetParent(transform.parent);
            dragPhantom.transform.localScale = Vector3.one;

            // removing background from phantom item
            dragPhantom.transform.GetChild(0).gameObject.SetActive(false);

            // disabling raycast
            foreach (var image in dragPhantom.GetComponentsInChildren<Image>())
            {
                image.raycastTarget = false;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(dragPhantom);
            dragPhantom = null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragPhantom.transform.position = Input.mousePosition;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var dropped = eventData.pointerDrag;
            var sourceSlot = dropped.GetComponent<SlotView>().slot;
            //container.Move(sourceSlot, slot);
        }
    }
}
