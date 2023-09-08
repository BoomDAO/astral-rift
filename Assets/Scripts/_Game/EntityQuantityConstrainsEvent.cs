using UnityEngine;
using UnityEngine.Events;

public class EntityQuantityConstrainsEvent : MonoBehaviour
{
    public enum QuantityConstrainType { EqualOrGreaterThan, LowerThan }
    public enum ExecutionType { OnStart, OnEnable, ExternalCall }
    [System.Serializable]
    public struct QuantityConstrain
    {
        public string eid, gid, wid;
        public QuantityConstrainType quantityConstrainType;
        public double valueToCompareTo;

        public readonly bool Validate()
        {
            var quantity = EntityUtil.GetCurrentQuantity($"{(string.IsNullOrEmpty(wid) ? Env.CanisterIds.WORLD : wid)}{gid}{eid}");

            if (quantityConstrainType == QuantityConstrainType.EqualOrGreaterThan)
            {
                Debug.Log($"> > > quantity >= valueToCompareTo = {(quantity >= valueToCompareTo)}");
                return quantity >= valueToCompareTo;

            }
            else
            {
                Debug.Log($"> > >quantity < valueToCompareTo = {(quantity < valueToCompareTo)}");

                return quantity < valueToCompareTo;
            }
        }
    }

    [SerializeField]
    ExecutionType executionType;
    [SerializeField] 
    bool trueForAll = true;
    [SerializeField]
    QuantityConstrain[] quantityConstrains;
    [SerializeField] UnityEvent onTrue, onFalse;


    private void Start()
    {
        if (executionType == ExecutionType.OnStart) _Check();
    }
    private void OnEnable()
    {
        if (executionType == ExecutionType.OnEnable) _Check();
    }
    public void Check()
    {
        if (executionType == ExecutionType.ExternalCall) _Check();
    }
    private void _Check()
    {
        Debug.Log($"> > > {gameObject.name}");

        if (trueForAll)
        {
            foreach (var item in quantityConstrains)
            {
                if (!item.Validate())
                {
                    onFalse.Invoke();
                    return;
                }
            }
            onTrue.Invoke();
        }
        else
        {
            foreach (var item in quantityConstrains)
            {
                if (item.Validate())
                {
                    onTrue.Invoke();
                    return;
                }
            }
            onFalse.Invoke();
        }
    }
}
