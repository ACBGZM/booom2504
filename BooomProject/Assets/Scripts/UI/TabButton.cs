using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public TabGroup tabGroup;
        public Image tabImage;
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;

        private void Awake() {
            tabImage = GetComponent<Image>();
            if (tabGroup == null) {
                tabGroup = GetComponentInParent<TabGroup>();
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            tabGroup?.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            tabGroup?.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData) {
            tabGroup?.OnTabExit(this);
        }

        public void Selected() {
            onTabSelected?.Invoke();
        }

        public void Deselected() {
            onTabDeselected?.Invoke();
        }
    }
}