using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using static UI.TabGroup;
using static UnityEngine.Rendering.DebugUI;

namespace UI {

    public class TabGroup : MonoBehaviour {

        [System.Serializable]
        public class TabPagePair {
            public TabButton button;
            public GameObject page;
        }

        [SerializeField] private List<TabPagePair> _tabPagePairs = new List<TabPagePair>();
        [SerializeField] private Color _tabIdle = Color.white;
        [SerializeField] private Color _tabActive = Color.cyan;
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _hoverScale = 1.2f; // 鼠标悬停放大倍数
        [SerializeField] private float _selectedScale = 1.1f; // 选中状态放大倍数
        private TabButton _tabSelected;

        // 按钮与原本大小
        private Dictionary<TabButton, Vector3> _buttonOriginalScales = new Dictionary<TabButton, Vector3>();

        private Dictionary<TabButton, Coroutine> _activeCoroutines = new Dictionary<TabButton, Coroutine>();

        private void Awake() {
            foreach (var pair in _tabPagePairs) {    // 初始化按钮大小
                if (pair.button != null) {
                    _buttonOriginalScales[pair.button] = pair.button.transform.localScale;
                }
            }
        }

        private void Start() {
            if (_tabPagePairs.Count > 0) {
                OnTabSelected(_tabPagePairs[0].button);
            }
        }

        public void OnTabEnter(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;
            if (_activeCoroutines.TryGetValue(button, out Coroutine existingCoroutine)) {
                StopCoroutine(existingCoroutine);
            }
            Vector3 targetScale = _buttonOriginalScales[button] * _hoverScale;
            _activeCoroutines[button] = StartCoroutine(AnimateProperty(
                button.transform.localScale,
                targetScale,
                (value) => button.transform.localScale = value,
                button
            ));
        }

        public void OnTabExit(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;
            if (_activeCoroutines.TryGetValue(button, out Coroutine existingCoroutine)) {
                StopCoroutine(existingCoroutine);
            }
            Vector3 targetScale = _tabSelected == button ?
                _buttonOriginalScales[button] * _selectedScale :
                _buttonOriginalScales[button];
            _activeCoroutines[button] = StartCoroutine(AnimateProperty(
                button.transform.localScale,
                targetScale,
                (value) => button.transform.localScale = value,
                button
            ));
        }

        public void OnTabSelected(TabButton button) {
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
                Vector3 targetScale = isSelected ?
                _buttonOriginalScales[pair.button] * _selectedScale :
                _buttonOriginalScales[button];

                Color targetColor = isSelected ? _tabActive : _tabIdle;
                // 设置选中按钮的缩放
                StartCoroutine(AnimateProperty(
                    pair.button.transform.localScale,
                    targetScale,
                    (value) => pair.button.transform.localScale = value,
                    pair.button
                ));

                StartCoroutine(AnimateProperty(
                    pair.button.tabImage.color,
                    targetColor,
                    (value) => pair.button.tabImage.color = value,
                    pair.button
                ));
            }
        }
        // 泛型支持Vector3 和 Color 将属性从 startValue 逐渐过渡到 endValue，并通过 setter 委托应用变化
        private IEnumerator AnimateProperty<T>(T startValue, T endValue, System.Action<T> setter, TabButton button) {
            // 记录动画已经过去的时间
            float elapsed = 0f;
            while (elapsed < _animationDuration) {
                elapsed += Time.deltaTime;
                // 计算插值 t
                float t = _scaleCurve.Evaluate(elapsed / _animationDuration);
                // 判断泛型 T 的类型
                if (typeof(T) == typeof(Vector3)) {
                    // 使用 Vector3.Lerp 进行线性插值
                    Vector3 current = Vector3.Lerp(
                        (Vector3)(object)startValue,
                        (Vector3)(object)endValue,
                        t
                    );
                    // 调用 setter 委托更新属性值
                    setter((T)(object)current);
                } else if (typeof(T) == typeof(Color)) {
                    // 使用 Color.Lerp 进行线性插值
                    Color current = Color.Lerp(
                        (Color)(object)startValue,
                        (Color)(object)endValue,
                        t
                    );
                    setter((T)(object)current);
                }
                yield return null;
            }
            // 确保属性值设置为 endValue
            setter(endValue);
            _activeCoroutines.Remove(button);
        }

    }
}
