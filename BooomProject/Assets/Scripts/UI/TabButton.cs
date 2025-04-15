using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UI
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public TabGroup tabGroup;
        public Image background;
        public int index;
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;
        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }

        private void Start()
        {
            background = GetComponent<Image>();
            
            tabGroup.Subscribe(this);
            index = transform.GetSiblingIndex();
        }

        public void Selected()
        {
            if(onTabSelected != null) onTabSelected.Invoke();
        }
        public void Deselected()
        {
            if(onTabDeselected != null) onTabDeselected.Invoke();
        }
    }
}

