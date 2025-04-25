using UnityEngine;

[CreateAssetMenu(fileName = "EventPlayAnimation", menuName = "Event/GP/Play Animation")]
public class EventPlayAnimation : EventNodeBase
{
    [SerializeField] private string m_go_name;
    [SerializeField] private string m_trigger_name;

    public override void Execute()
    {
        base.Execute();

        GameObject go = GameObject.Find(m_go_name);
        int m_animation_hash = Animator.StringToHash(m_trigger_name);
        go.GetComponent<Animator>().SetTrigger(m_animation_hash);

        m_on_finished?.Invoke(true);
    }
}
