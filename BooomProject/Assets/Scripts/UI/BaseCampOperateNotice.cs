using TMPro;
using UnityEngine;

public class BaseCampOperateNotice : MonoBehaviour {
    [SerializeField] private GameObject _NPCNotice;
    [SerializeField] private GameObject _ExitNotice;
    [SerializeField] private PlayerController _controller;
    private TextMeshProUGUI _noticeText;

    private void Awake() {
        _NPCNotice.SetActive(false);
        _ExitNotice.SetActive(false);
        _controller.OnInteractableObjectChange += PlayerController_OnInteractableObjectChange;
    }

    private void PlayerController_OnInteractableObjectChange(IInteractable interactable, bool isEnter) {
        if (isEnter) {
            if(interactable is NPCInteract) _NPCNotice.SetActive(true);
            else if(interactable is CustomInteract
                    && !CommonGameplayManager.GetInstance().TimeManager.IsOffWork()) _ExitNotice.SetActive(true);
        } else {
            _NPCNotice.SetActive(false);
            _ExitNotice.SetActive(false);
        }
    }

    private void OnDestroy() {
        _controller.OnInteractableObjectChange -= PlayerController_OnInteractableObjectChange;
    }
}
