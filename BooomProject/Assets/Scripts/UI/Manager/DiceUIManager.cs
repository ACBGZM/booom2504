using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceUIManager : Singleton<DiceUIManager>
{
  //  public Button rollBtn;
    public TMP_Text result;
  
    public int val;

    public TMP_Text speed;
    public TMP_Text reputation;

    public GameObject renderCanvas;
    protected override void init()
    {
        //   rollBtn = gameObject.GetComponentInChildren<Button>();
        Transform child = transform.GetChild(0);
        result = child.Find("result").GetComponentInChildren<TMP_Text>();


        speed = child.Find("speed").GetComponentInChildren<TMP_Text>();

        reputation = child.Find("reputation").GetComponentInChildren<TMP_Text>();
        renderCanvas = transform.parent.Find("Canvas").gameObject;
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
        reputation.text = $"声誉：{CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value.ToString()}";
        renderCanvas.SetActive(true);
        gameObject.SetActive(true);
        

        EventHandlerManager.CallStartRoll();
      //  rollBtn.gameObject.SetActive(true);
    }

    public void HideMe()
    {
        gameObject.SetActive(false);
        renderCanvas.SetActive(false);
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
