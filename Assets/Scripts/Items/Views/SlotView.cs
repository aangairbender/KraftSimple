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
        [SerializeField] private Image itemImage;
        [SerializeField] private GameObject quantityRoot;
        [SerializeField] private TextMeshProUGUI quantityCaption;

        private Item item;
        private int quantity;

        public Item Item
        {
            get => item;
            set
            {
                item = value;
                PresentSlot();
            }
        }
        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                PresentSlot();
            }
        }

        private GameObject dragPhantom;

        private void PresentSlot()
        {
            itemImage.sprite = item?.Data?.Sprite;
            quantityCaption.text = quantity.ToString();
            quantityRoot.SetActive(quantity > 1);

            itemImage.gameObject.SetActive(item != null);
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
            //var sourceSlot = dropped.GetComponent<SlotView>().slot;
            //container.Move(sourceSlot, slot);
        }
    }
}
