using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;

        public Image _tabImage;

        [SerializeField] public Transform _iconTransform; // 原图标变换组件
        [SerializeField] private Color _iconActiveColor = Color.white;
        [SerializeField] private Color _iconInactiveColor = Color.gray;

        private TabGroup _tabGroup;
        private Vector3 _iconOriginalScale;

        private void Awake() {
            if (_tabImage == null) _tabImage = GetComponentInChildren<Image>();
            // 初始化图标原始尺寸
            if (_iconTransform != null) _iconOriginalScale = _iconTransform.localScale;
            // 查找父级TabGroup
            if (_tabGroup == null) _tabGroup = GetComponentInParent<TabGroup>();
            var parentImage = GetComponent<Image>();
            if (parentImage != null) {
                parentImage.raycastTarget = true;
                parentImage.color = new Color(0, 0, 0, 0); // 完全透明
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            _tabGroup?.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _tabGroup?.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData) {
            _tabGroup?.OnTabExit(this);
        }
    }
}
