using System;
using System.Collections;
using UnityEngine;

public class DeliveryPlayer : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveDuration = 0f;
    [SerializeField] private bool _useSpeed = true;
    
    public enum DeliveryPlayerState
    {
        Idle,
        Moving,
    }
    [SerializeField] private DeliveryPlayerState _state = DeliveryPlayerState.Idle;

    public void Start()
    {
        transform.position = GameManager.Instance.NodeGraphManager.CurrentNode.transform.position;
    }

    public bool Move(Vector3[] path, Action onComplete = null)
    {
        if (_state == DeliveryPlayerState.Moving)
        {
            return false;
        }

        if (path == null || path.Length < 2)
        {
            return false;
        }

        StopAllCoroutines();    // todo
        StartCoroutine(MoveAlongPath(path, onComplete));

        return true;
    }

    private IEnumerator MoveAlongPath(Vector3[] path, Action onComplete = null)
    {
        _state = DeliveryPlayerState.Moving;
        
        float totalDistance = 0f;
        for (int i = 1; i < path.Length; i++)
        {
            totalDistance += Vector2.Distance(path[i - 1], path[i]);    // vec3 -> vec2
        }

        float totalTime = _useSpeed ? totalDistance / _moveSpeed : _moveDuration;
        if (totalTime <= 0)
        {
            _state = DeliveryPlayerState.Idle;
            yield break;
        }

        StartMoving();
        
        int currentSegment = 0;

        while (currentSegment < path.Length - 1)
        {
            float t = 0f;
            Vector2 begin = path[currentSegment];
            Vector2 end = path[currentSegment + 1];
            float segmentDistance = Vector2.Distance(begin, end);
            float segmentDuration =  _useSpeed ?
                (segmentDistance / _moveSpeed)
                : (segmentDistance / totalDistance * totalTime);

            while (t < 1f)
            {
                t += Time.deltaTime / segmentDuration;
                transform.position = Vector2.Lerp(begin, end, Mathf.Clamp01(t));
                yield return null;
            }

            ++currentSegment;
        }

        transform.position = path[^1];
        
        onComplete?.Invoke();
        
        StopMoving();
        
        _state = DeliveryPlayerState.Idle;
    }
    
    private void StartMoving()
    {
    }

    private void StopMoving()
    {
    }
}
