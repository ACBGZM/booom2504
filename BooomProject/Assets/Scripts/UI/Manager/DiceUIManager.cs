using TMPro;
using UnityEngine.UI;

public class DiceUIManager : Singleton<DiceUIManager>
{
    public Button rollBtn;
    public TMP_Text result;
    public int val;

    protected override void init()
    {
        rollBtn = gameObject.GetComponentInChildren<Button>();
        result = gameObject.GetComponentInChildren<TMP_Text>();
        rollBtn.onClick.AddListener(() =>
        {
            rollBtn.gameObject.SetActive(false);
        });
    }

    private void OnEnable()
    {
        EventHandlerManager.rollFinish += OnRollFinish;
    }

    private void OnDisable()
    {
        EventHandlerManager.rollFinish -= OnRollFinish;
    }

    private void Start()
    {
        HideMe();
    }

    public void ShowMe()
    {
        Reset();
        gameObject.SetActive(true);
        rollBtn.gameObject.SetActive(true);
    }

    public void HideMe()
    {
        gameObject.SetActive(false);
        Reset();
    }

    public void OnRollFinish(int val)
    {
        this.val = val;
        result.text = val.ToString();
    }

    public void Reset()
    {
        val = 0;
        result.text = string.Empty;
    }
}
