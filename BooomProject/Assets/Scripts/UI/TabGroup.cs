using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TabGroup : MonoBehaviour
    {
        [System.Serializable]
        public class TabPagePair
        {
            public TabButton button;
            public GameObject page;
        }

        [SerializeField] private List<TabPagePair> _tabPagePairs = new List<TabPagePair>();
        [SerializeField] private Color _tabIdle = Color.white;
        [SerializeField] private Color _tabActive = Color.cyan;
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _hoverScale = 1.2f; // 鼠标悬停放大倍数
        [SerializeField] public float _selectedScale = 1.1f; // 选中状态放大倍数
        private TabButton _tabSelected;

        // 按钮与原本大小
        private Dictionary<TabButton, Vector3> _buttonOriginalScales = new Dictionary<TabButton, Vector3>();
        private Dictionary<TabButton, Vector3> _iconOriginalScales = new Dictionary<TabButton, Vector3>();
        private Dictionary<TabButton, Coroutine> _activeCoroutines = new Dictionary<TabButton, Coroutine>();

        private void Awake() {
            foreach (var pair in _tabPagePairs) {
                if (pair.button != null) {
                    // 记录按钮的原始缩放
                    _buttonOriginalScales[pair.button] = pair.button.transform.localScale;
                    // 记录图标的原始缩放
                    if (pair.button._iconTransform != null) {
                        _iconOriginalScales[pair.button] = pair.button._iconTransform.localScale;
                    }
                }
            }
        }

        public void OnTabEnter(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;

            if (button._iconTransform != null) {
                if(button != _tabSelected) AnimateIconScale(button, _iconOriginalScales[button] * _hoverScale);
            }
        }

        public void OnTabExit(TabButton button) {
            if (button._iconTransform != null) {
                var targetScale = _tabSelected == button ?
                    _iconOriginalScales[button] * _selectedScale :
                    _iconOriginalScales[button];

                if (button != _tabSelected) AnimateIconScale(button, targetScale);
            }
        }

        private void AnimateIconScale(TabButton button, Vector3 targetScale) {
            if (_activeCoroutines.TryGetValue(button, out Coroutine existing)) {
                StopCoroutine(existing);
            }
            _activeCoroutines[button] = StartCoroutine(AnimateProperty(
                button._iconTransform.localScale,
                targetScale,
                value => button._iconTransform.localScale = value,
                button
            ));
        }

        private void Start()
        {
            if (_tabPagePairs.Count > 0)
            {
                OnTabSelected(_tabPagePairs[0].button);
            }
        }

        public void OnTabSelected(TabButton button) {
            // 停止所有正在进行的动画
            foreach (var pair in _tabPagePairs) {
                if (_activeCoroutines.TryGetValue(pair.button, out Coroutine coroutine)) {
                    StopCoroutine(coroutine);
                    _activeCoroutines.Remove(pair.button);
                }
            }

            _tabSelected = button;

            foreach (var pair in _tabPagePairs) {
                bool isSelected = pair.button == button;
                pair.page.SetActive(isSelected);

                // 使用当前按钮的原始缩放计算目标
                Vector3 targetScale = isSelected
                    ? _buttonOriginalScales[pair.button] * _selectedScale
                    : _buttonOriginalScales[pair.button];

                Color targetColor = isSelected ? _tabActive : _tabIdle;

                // 动画按钮缩放和颜色
                StartCoroutine(AnimateProperty(
                    pair.button.transform.localScale,
                    targetScale,
                    value => pair.button.transform.localScale = value,
                    pair.button
                ));
                StartCoroutine(AnimateProperty(
                    pair.button._tabImage.color,
                    targetColor,
                    value => pair.button._tabImage.color = value,
                    pair.button
                ));
            }
        }

        private IEnumerator AnimateProperty(Vector3 startValue, Vector3 endValue, System.Action<Vector3> setter, TabButton button) {
            float elapsed = 0f;
            while (elapsed < _animationDuration) {
                elapsed += Time.deltaTime;
                float t = _scaleCurve.Evaluate(elapsed / _animationDuration);
                setter(Vector3.Lerp(startValue, endValue, t));
                yield return null;
            }
            setter(endValue);
            _activeCoroutines.Remove(button);
        }

        private IEnumerator AnimateProperty(Color startValue, Color endValue, System.Action<Color> setter, TabButton button) {
            float elapsed = 0f;
            while (elapsed < _animationDuration) {
                elapsed += Time.deltaTime;
                float t = _scaleCurve.Evaluate(elapsed / _animationDuration);
                setter(Color.Lerp(startValue, endValue, t));
                yield return null;
            }
            setter(endValue);
            _activeCoroutines.Remove(button);
        }
    }
}
