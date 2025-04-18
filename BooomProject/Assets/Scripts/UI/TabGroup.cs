using UnityEngine;
using System.Collections.Generic;

namespace UI {
    public class TabGroup : MonoBehaviour {
        [System.Serializable]
        public class TabPagePair {
            public TabButton button;
            public GameObject page;
        }

        public List<TabPagePair> tabPagePairs = new List<TabPagePair>();
        //public Color tabHover = Color.cyan;
        public Color tabIdle = Color.white;
        public Color tabActive = Color.cyan;
        private TabButton _tabSelected;

        private float hobverScale = 1.2f; // 鼠标悬停放大倍数
        private Vector3 buttonOriginalScale; // 按钮原本大小

        private void Awake()
        {
            buttonOriginalScale = tabPagePairs[0].button.transform.localScale; // 初始化按钮大小
        }

        private void Start() {
            if (tabPagePairs.Count > 0) {
                OnTabSelected(tabPagePairs[0].button);
            }
        }

        public void OnTabEnter(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;
            button.transform.localScale = buttonOriginalScale * hobverScale;
        }

        public void OnTabExit(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;
            button.tabImage.color = tabIdle;
            button.transform.localScale = buttonOriginalScale;
        }

        public void OnTabSelected(TabButton button) {
            // Deselect old
            if (_tabSelected != null) {
                _tabSelected.tabImage.color = tabIdle;
                _tabSelected.transform.localScale = buttonOriginalScale;
                _tabSelected.Deselected();
            }

            // Select new
            _tabSelected = button;
            _tabSelected.transform.localScale = buttonOriginalScale;
            button.tabImage.color = tabActive;

            button.Selected();

            // Switch page
            foreach (var pair in tabPagePairs) {
                pair.page.SetActive(pair.button == button);
            }
        }
    }
}