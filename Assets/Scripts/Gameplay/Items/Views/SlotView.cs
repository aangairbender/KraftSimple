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

        public ItemContainerView ItemContainerView { get; set; }
        public int Index { get; set; }

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
            dragPhantom = Instantiate(gameObject, gameObject.transform.parent);
            var layoutElement = dragPhantom.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;
            dragPhantom.transform.position = Input.mousePosition;
            dragPhantom.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 64);
            dragPhantom.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 64);

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
            Debug.Log("end drag");
            Destroy(dragPhantom);
            dragPhantom = null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragPhantom.transform.position = Input.mousePosition;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("drop");
            var sourceSlotView = eventData.pointerDrag.GetComponent<SlotView>();
            ItemContainerView.Drag(sourceSlotView.Index, this.Index, sourceSlotView.quantity);
        }
    }
}
