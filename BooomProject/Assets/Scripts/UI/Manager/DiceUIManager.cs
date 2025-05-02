using System.Diagnostics;
using TMPro;
using UnityEngine.UI;

public class DiceUIManager : Singleton<DiceUIManager>
{
  //  public Button rollBtn;
    public TMP_Text result;
  
    public int val;

    public TMP_Text speed;
    public TMP_Text reputation;

    protected override void init()
    {
     //   rollBtn = gameObject.GetComponentInChildren<Button>();
        result = gameObject.GetComponentInChildren<TMP_Text>();
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        
        speed = images[1].GetComponentInChildren<TMP_Text>();
        
        reputation = images[2].GetComponentInChildren<TMP_Text>();
    
        //rollBtn.onClick.AddListener(() =>
        //{
        //    rollBtn.gameObject.SetActive(false);
        //});
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
        speed.text = $"速度：{CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value.ToString()}";
        reputation.text = $"速度：{CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value.ToString()}";
        gameObject.SetActive(true);

        EventHandlerManager.CallStartRoll();
      //  rollBtn.gameObject.SetActive(true);
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
