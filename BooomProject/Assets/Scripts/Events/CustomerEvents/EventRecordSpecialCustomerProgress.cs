using UnityEngine;

[CreateAssetMenu(fileName = "RecordSpecialCustomerProgress", menuName = "Event/Customer/RecordSpecialCustomerProgress")]
public class EventRecordSpecialCustomerProgress : EventNodeBase
{
    [SerializeField] CustomerSO _customer;

    public override void Execute()
    {
        base.Execute();
        if (!CommonGameplayManager.GetInstance().SpecialCustomerProgress.TryAdd(_customer.customerName, 1))
        {
            ++CommonGameplayManager.GetInstance().SpecialCustomerProgress[_customer.customerName];
        }

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
