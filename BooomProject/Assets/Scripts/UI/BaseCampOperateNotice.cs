using TMPro;
using UnityEngine;

public class BaseCampOperateNotice : MonoBehaviour {
    [SerializeField] private GameObject Notice;
    [SerializeField] private PlayerController _controller;
    private TextMeshProUGUI _noticeText;

    private void Awake() {
        Notice.SetActive(false);
        _noticeText = Notice.GetComponentInChildren<TextMeshProUGUI>();
        _controller.OnInteractableObjectChange += PlayerController_OnInteractableObjectChange;
    }

    private void PlayerController_OnInteractableObjectChange(IInteractable interactable, bool isEnter) {
        if (isEnter) {
            if (interactable is NPCInteract) {
                Notice.SetActive(true);
                _noticeText.text = $"与 {interactable.GetInteractableName()} 交谈";
            }else{
                Notice.SetActive(true);
                _noticeText.text = interactable.GetInteractableName();
            }
        } else {
            var current = _controller.currentInteractable;
            if (current != null && (current is NPCInteract || current is CustomInteract)) {
                _noticeText.text = current.GetInteractableName();
            } else {
                Notice.SetActive(false);
            }
        }
    }

    private void OnDestroy() {
        _controller.OnInteractableObjectChange -= PlayerController_OnInteractableObjectChange;
    }
}
