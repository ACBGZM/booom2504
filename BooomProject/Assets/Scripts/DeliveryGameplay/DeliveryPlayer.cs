using System;
using System.Collections;
using UnityEngine;

public class DeliveryPlayer : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveDuration = 0f;
    [SerializeField] private bool _useSpeed = true;

    private DeliveryPlayerVisual _deliveryPlayerVisual;

    void Start()
    {
        transform.position = CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.transform.position;

        _deliveryPlayerVisual = GetComponentInChildren<DeliveryPlayerVisual>();
        _deliveryPlayerVisual.PlayIdleAnimation();
    }

    public bool Move(Vector3[] path, Action onComplete = null)
    {
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
        float totalDistance = 0f;
        for (int i = 1; i < path.Length; i++)
        {
            totalDistance += Vector2.Distance(path[i - 1], path[i]);    // vec3 -> vec2
        }

        float totalTime = _useSpeed ? totalDistance / CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value : _moveDuration;
        if (totalTime <= 0)
        {
            yield break;
        }

        StartMoving();

        for (int currentSegment = 0; currentSegment < path.Length - 1; ++currentSegment)
        {
            float t = 0f;
            Vector2 begin = path[currentSegment];
            Vector2 end = path[currentSegment + 1];

            if(end.x < begin.x)
            {
                _deliveryPlayerVisual.SetFlip(true);
            }
            else
            {
                _deliveryPlayerVisual.SetFlip(false);
            }

            float segmentDistance = Vector2.Distance(begin, end);
            float segmentDuration = _useSpeed ?
                (segmentDistance / CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value)
                : (segmentDistance / totalDistance * totalTime);
            int nodeId = CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID;
            int tryNodeId = CommonGameplayManager.GetInstance().NodeGraphManager.TryToNode.NodeID;
            float distance = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(tryNodeId, nodeId);

            // TODO：速度待优化
            float costTime = distance / (CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value * 0.3f);
           
            int minute = (int)costTime;
          
            CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(minute / segmentDuration);
            while (t < 1f)
            {
                t += Time.deltaTime / segmentDuration;
                transform.position = Vector2.Lerp(begin, end, Mathf.Clamp01(t));
                yield return null;
            }
        }

        transform.position = path[^1];

        StopMoving();

        onComplete?.Invoke();
    }

    private void StartMoving()
    {
        CommonGameplayManager.GetInstance().PlayerState = EPlayerState.PlayerMoving;
        _deliveryPlayerVisual.PlayMoveAnimation();
    }

    private void StopMoving()
    {
        CommonGameplayManager.GetInstance().PlayerState = EPlayerState.PlayerIdle;
        _deliveryPlayerVisual.PlayIdleAnimation();
    }
}
