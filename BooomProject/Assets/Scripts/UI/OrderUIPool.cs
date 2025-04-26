using UnityEngine;
using System.Collections.Generic;

public class OrderUIPool {
    private Dictionary<GameObject, Queue<Transform>> _pool = new Dictionary<GameObject, Queue<Transform>>();
    private Dictionary<Transform, GameObject> _prefabLookup = new Dictionary<Transform, GameObject>();

    public Transform Get(Transform prefab, Transform parent) {
        GameObject prefabGO = prefab.gameObject;

        if (!_pool.ContainsKey(prefabGO)) {
            _pool.Add(prefabGO, new Queue<Transform>());
        }

        Transform item;
        if (_pool[prefabGO].Count > 0) {
            item = _pool[prefabGO].Dequeue();
            item.SetParent(parent, false); // 设置父级
            item.gameObject.SetActive(true);
        } else {
            item = UnityEngine.Object.Instantiate(prefab, parent);
            _prefabLookup.Add(item, prefabGO); // 存储实例来自哪个预制件
        }

        item.SetAsLastSibling(); // 出现在底部
        return item;
    }

    public void Return(Transform item) {
        if (item == null || !item.gameObject.activeSelf) return;

        item.gameObject.SetActive(false);
        item.SetParent(null);

        if (_prefabLookup.TryGetValue(item, out GameObject prefabGO)) {
            if (!_pool.ContainsKey(prefabGO)) {
                _pool.Add(prefabGO, new Queue<Transform>());
                Debug.LogWarning($"池中不包含 {prefabGO.name} 的键。");
            }
            _pool[prefabGO].Enqueue(item);
        } else {
            Debug.LogError($"无法找到退回的 {item.name} ");
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }

    public void ClearPool()
    {
        foreach (var queue in _pool.Values) {
            while (queue.Count > 0) {
                var item = queue.Dequeue();
                if (item != null) UnityEngine.Object.Destroy(item.gameObject);
            }
        }
        _pool.Clear();
        _prefabLookup.Clear();
    }
}
