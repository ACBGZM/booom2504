using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseCampDayUI : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    private void Start() {
        dayText = GetComponent<TextMeshProUGUI>();
        CommonGameplayManager.GetInstance().TimeManager.OnDayPassed.AddListener(UpdateDayUI);
    }

    private void UpdateDayUI(GameTime current) {
        dayText.text = current.GetDay();
    }
}
