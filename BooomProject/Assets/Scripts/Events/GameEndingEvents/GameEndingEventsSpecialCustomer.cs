using UnityEngine;

[CreateAssetMenu(fileName = "GameEndingEventsSpecialCustomer", menuName = "Event/GP/Game Ending - SpecialCustomer")]
public class GameEndingEventsSpecialCustomer : EventNodeBase
{
    public CustomerSO Customer;
    public EventShowDialogue CustomerEnding1;
    public EventShowDialogue CustomerEnding2;
    public EventShowDialogue CustomerEnding3;
    public EventShowDialogue CustomerEnding4;

    public override void Execute()
    {
        base.Execute();

        if (!CommonGameplayManager.GetInstance().SpecialCustomerProgress.ContainsKey(Customer.customerName))
        {
            CustomerEnding1.Initialize(OnFinish);
            CustomerEnding1.Execute();
        }
        else if (CommonGameplayManager.GetInstance().SpecialCustomerProgress[Customer.customerName] == 1)
        {
            CustomerEnding2.Initialize(OnFinish);
            CustomerEnding2.Execute();
        }
        else if (CommonGameplayManager.GetInstance().SpecialCustomerProgress[Customer.customerName] == 2)
        {
            CustomerEnding3.Initialize(OnFinish);
            CustomerEnding3.Execute();
        }
        else if (CommonGameplayManager.GetInstance().SpecialCustomerProgress[Customer.customerName] == 3)
        {
            CustomerEnding4.Initialize(OnFinish);
            CustomerEnding4.Execute();
        }
    }

    public void OnFinish(bool success)
    {
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
