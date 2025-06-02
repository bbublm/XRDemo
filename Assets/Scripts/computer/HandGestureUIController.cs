using UnityEngine;

public class HandGestureUIController : MonoBehaviour
{
    public OVRHand hand;                    // 分配左手或右手
    public GameObject uiPanel;              // 要显示/隐藏的UI面板
    public float pinchThreshold = 0.5f;
    public float releaseThreshold = 0.3f;
    public Vector3 uiOffset = new Vector3(0.1f, 0f, 0.1f); // UI 相对手的位置偏移
    public float activationDistance = 0.1f; // 激活距离
    private Vector3 originalScale;

    private bool isHoldingUI = false;

    void Start()
    {
        originalScale = uiPanel.transform.localScale;
    }

    void Update()
    {
        if (hand == null || !hand.IsTracked) return;

        bool isMakingFist = IsMakingFist(hand);
        bool isHandOpen = IsHandOpen(hand);

        float handToUI = Vector3.Distance(hand.transform.position, uiPanel.transform.position);
        Debug.Log("handToUI: " + handToUI);

        if (isMakingFist && !isHoldingUI && handToUI < activationDistance)
        {
            AttachUIToHand();
        }
        else if (isHandOpen && isHoldingUI)
        {
            // 2. 放下 UI
            DetachUI();
        }

        // 如果正在吸附，持续跟随
        if (isHoldingUI && uiPanel.activeSelf)
        {
            FollowHand();
        }
    }

    bool IsMakingFist(OVRHand h)
    {
        return h.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold &&
               h.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchThreshold &&
               h.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) > pinchThreshold;
    }

    bool IsHandOpen(OVRHand h)
    {
        return h.GetFingerPinchStrength(OVRHand.HandFinger.Index) < releaseThreshold &&
               h.GetFingerPinchStrength(OVRHand.HandFinger.Middle) < releaseThreshold &&
               h.GetFingerPinchStrength(OVRHand.HandFinger.Ring) < releaseThreshold &&
               h.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) < releaseThreshold;
    }

    void AttachUIToHand()
    {
        isHoldingUI = true;
        uiPanel.SetActive(true);
    }

    void FollowHand()
    {
        // 1. 位置：跟随手部位置 + 偏移
        uiPanel.transform.position = hand.transform.position + hand.transform.rotation * uiOffset;

        // 2. 缩放为原始的一半
        uiPanel.transform.localScale = originalScale * 0.5f;

        // 3. 姿态：正面朝向玩家摄像机
        Transform cameraTransform = Camera.main.transform;
        Vector3 lookDirection = uiPanel.transform.position - cameraTransform.position;
        lookDirection.y = 0; // 保持水平朝向（可根据需要移除）
        
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            uiPanel.transform.forward = lookDirection.normalized;
        }
    }


    void DetachUI()
    {
        uiPanel.transform.localScale = originalScale;
        isHoldingUI = false;
        // UI 保持在当前位置
    }
}
