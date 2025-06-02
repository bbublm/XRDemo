using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

public class TwoHandPullWithTouchGrab : Grabbable
{
    public float triggerDistance = 0.05f; // 5cm
    private bool eventTriggered = false;

    public delegate void DistanceTriggerEvent();
    public event DistanceTriggerEvent OnDistanceExceeded;

    void Update()
    {
        base.UpdateTransform();

        Debug.Log("GrabPoints.Count: " + GrabPoints.Count);

        // 只有两个抓取点时才检测
        if (GrabPoints.Count == 2)
        {
            float dist = Vector3.Distance(GrabPoints[0].position, GrabPoints[1].position);
            if (!eventTriggered && dist > triggerDistance)
            {
                eventTriggered = true;
                OnDistanceExceeded?.Invoke();
                Debug.Log($"两手距离超过 {triggerDistance * 100f} 厘米，触发事件");
            }
            else if (eventTriggered && dist <= triggerDistance)
            {
                // 如果需要事件复位，也可以在这里处理
                eventTriggered = false;
                Debug.Log("两手距离回到阈值内，事件复位");
            }
        }
        else
        {
            // 抓取点不足2个，重置事件状态
            eventTriggered = false;
        }
    }
}