using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 通用数值类
/// </summary>
[System.Serializable]
public class BaseStat<T> where T : struct {
    [SerializeField] private string _statName;
    [SerializeField] private T _value;
    [SerializeField] private T _minValue;
    [SerializeField] private T _maxValue;

    public UnityEvent<T> OnValueChanged = new UnityEvent<T>();
    public T minValue => _minValue;
    public T maxValue => _maxValue;

    public BaseStat(string name, T initial, T min, T max) {
        _statName = name;
        _value = ClampValue(initial);
        _minValue = min;
        _maxValue = max;
    }

    public T Value {
        get => _value;
        set {
            T newVal = ClampValue(value);
            if (!newVal.Equals(_value)) {
                _value = newVal;
                OnValueChanged.Invoke(_value);
            }
        }
    }

    private T ClampValue(T input) {
        if (input is float f) {
            return (T)(object)Mathf.Clamp(f, (float)(object)_minValue, (float)(object)_maxValue);
        }
        if (input is int i) {
            return (T)(object)Mathf.Clamp(i, (int)(object)_minValue, (int)(object)_maxValue);
        }
        return input;
    }

    public void Add(T amount) => Value = AddValues(_value, amount);
    public void Reset() => Value = _minValue;

    private static dynamic AddValues(dynamic a, dynamic b) => a + b;
}
