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
        public Color tabHover = Color.gray;
        public Color tabIdle = Color.white;
        public Color tabActive = Color.blue;
        private TabButton _tabSelected;

        private void Start() {
            if (tabPagePairs.Count > 0) {
                OnTabSelected(tabPagePairs[0].button);
            }
        }

        public void OnTabEnter(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;
            button.tabImage.color = tabHover;
        }

        public void OnTabExit(TabButton button) {
            if (_tabSelected != null && _tabSelected == button) return;
            button.tabImage.color = tabIdle;
        }

        public void OnTabSelected(TabButton button) {
            // Deselect old
            if (_tabSelected != null) {
                _tabSelected.tabImage.color = tabIdle;
                _tabSelected.Deselected();
            }
            // Select new
            _tabSelected = button;
            button.tabImage.color = tabActive;
            button.Selected();
            // Switch page
            foreach (var pair in tabPagePairs) {
                pair.page.SetActive(pair.button == button);
            }
        }
    }
}