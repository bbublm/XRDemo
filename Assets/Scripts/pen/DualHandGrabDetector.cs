using UnityEngine;

public class DualHandGrabDetector : MonoBehaviour
{
    [Header("手部引用")]
    public OVRHand leftHand;
    public OVRHand rightHand;

    [Header("Pinch 判定阈值")]
    public float pinchThreshold = 0.9f;

    [Header("距离判定")]
    public float separationDistance = 0.2f; // 手部分开的距离（米）

    [Header("状态判断")]
    public bool leftGrabbing = false;
    public bool rightPinching = false;

    [Header("UI")]
    public GameObject uiPanel;

    private bool hasTriggered = false;

    void Update()
    {
        if (leftHand == null || rightHand == null) return;

        // 1. 读取左右手 pinch 状态
        leftGrabbing = CheckGrabWithMetaSDK(leftHand); // Meta SDK 的抓取
        rightPinching = IsPinching(rightHand);         // 自定义 Pinch 判定

        Debug.Log("leftGrabbing: " + leftGrabbing);
        Debug.Log("rightPinching: " + rightPinching);

        // 2. 两只手在接触状态时记录位置
        if (leftGrabbing && rightPinching && !hasTriggered)
        {
            float handDistance = Vector3.Distance(leftHand.transform.position, rightHand.transform.position);
            Debug.Log("Hand Distance: " + handDistance);

            if (handDistance >= separationDistance)
            {
                Debug.Log("触发 UI！");
                TriggerUI();
                hasTriggered = true;
            }
        }

        // 3. 重置状态（重新靠近才允许再触发）
        if (!(leftGrabbing && rightPinching))
        {
            hasTriggered = false;
        }
    }

    // ✅ 判断某只手是否 Pinch（Index + Thumb）
    private bool IsPinching(OVRHand hand)
    {
        return hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold &&
               hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) > pinchThreshold;
    }

    private bool CheckGrabWithMetaSDK(OVRHand hand)
    {
        // 柱体中心位置
        Vector3 center = this.transform.position;
        // 柱体上下端点
        float height = 0.1f;  // 你的柱体高度，记得根据实际模型调整
        Vector3 up = this.transform.up;

        Vector3 topPoint = center + up * (height / 2f);
        Vector3 bottomPoint = center - up * (height / 2f);

        // 计算手点到柱体轴线线段的距离
        float distanceToLineSegment = DistancePointToLineSegment(hand.transform.position, bottomPoint, topPoint);

        // 判定是否靠近
        bool isNear = distanceToLineSegment < 0.1f;  // 阈值根据需求调整


        Debug.Log("distanceToLineSegment: " + distanceToLineSegment);
        // 2. 握拳状态判断
        bool isFist = IsMakingFist(hand);

        Debug.Log("isNear: " + isNear);
        Debug.Log("isFist: " + isFist);

        return isNear && isFist;
    }

    private bool IsMakingFist(OVRHand hand)
    {
        Debug.Log("hand.GetFingerPinchStrength(OVRHand.HandFinger.Index): " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Index));
        Debug.Log("hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle): " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle));
        Debug.Log("hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring): " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring));
        Debug.Log("hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky): " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky));
        Debug.Log("hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb): " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb));

        return hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.4f &&
            hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.5f &&
            hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) > 0.5f;
    }

    float DistancePointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDir = lineEnd - lineStart;
        float lineLength = lineDir.magnitude;
        lineDir.Normalize();

        float projectLength = Vector3.Dot(point - lineStart, lineDir);
        projectLength = Mathf.Clamp(projectLength, 0f, lineLength);

        Vector3 closestPoint = lineStart + lineDir * projectLength;
        return Vector3.Distance(point, closestPoint);
    }


    private void TriggerUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }
}