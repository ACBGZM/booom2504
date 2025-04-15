using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TabGroup : MonoBehaviour
    {
        
        public List<TabButton> tabButtons;
      
        public List<GameObject> pages;
        // ��ť��ͣͼƬ
        public Sprite tabHover;
        // ��ťĬ��ͼƬ
        public Sprite tabIdle;
        // ��ťѡ��ͼƬ
        public Sprite tabActive;
        public TabButton tabSelected;

        public void Subscribe(TabButton button)
        {
            if (tabButtons == null)
            {
                tabButtons = new List<TabButton>();
            }
            tabButtons.Add(button);
        }

        public void OnTabEnter(TabButton button)
        {
            ResetTab();
            button.background.sprite = tabHover;
        }
        public void OnTabExit(TabButton button)
        {
            ResetTab();
        }

        public void OnTabSelected(TabButton button)
        {
            if(tabSelected != null)
            {
                button.Deselected();
            }
            tabSelected = button;
            button.Selected();
            ResetTab();
            for (int i = 0; i < pages.Count; i ++ )
            {
                pages[i].SetActive(i == button.index);               
            }
            button.background.sprite = tabActive;
        }

        public void ResetTab()
        {
            foreach (TabButton button in tabButtons)
            {
                if (tabSelected != null && tabSelected == button) continue;
                button.background.sprite = tabIdle;
            }
           
        }
    }
}

