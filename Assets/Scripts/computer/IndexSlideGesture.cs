using UnityEngine;

public class IndexSlideGesture : MonoBehaviour
{
    public OVRHand ovrHand;
    public GameObject uiPanel;
    public float slideThreshold = 0.05f; // 滑动触发距离

    private Vector3 lastIndexTipPos;
    private bool wasPinching = false;
    private bool indexTipInitialized = false;
    private OVRSkeleton skeleton;

    private float elapsedTime = 0f;
    private bool delayPassed = false;

    void Start()
    {
        skeleton = ovrHand.GetComponent<OVRSkeleton>();
    }

    void Update()
    {
        // 计时
        if (!delayPassed)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 16f)
            {
                delayPassed = true;
            }
            else
            {
                return; // 等待 16 秒前，不执行后续逻辑
            }
        }
        
        if (ovrHand == null || !ovrHand.IsTracked)
            return;

        if (!indexTipInitialized && skeleton.IsDataValid && skeleton.IsDataHighConfidence)
        {
            lastIndexTipPos = GetIndexTipWorldPos();
            indexTipInitialized = true;
            return;
        }

        if (!indexTipInitialized)
            return;

        Vector3 currentPos = GetIndexTipWorldPos();
        Debug.Log("currentPos: " + currentPos);
        float distance = Vector3.Distance(currentPos, lastIndexTipPos);

        Debug.Log("distanceFingerTip: " + distance);

        // 当手指“刚刚开始 pinch”，记录位置
        bool isPinching = IsOneGesture(ovrHand);
        Debug.Log("isPinching: " + isPinching);

        if (isPinching && !wasPinching)
        {
            lastIndexTipPos = currentPos;
        }

        Debug.Log("distanceFingerTip_last: " + distance);

        // 如果是持续 pinch 状态并移动距离超过阈值
        if (isPinching && wasPinching && distance > slideThreshold)
        {
            TriggerUI();
            wasPinching = false; // 重置，避免多次触发
            distance = 0;
        }
        else
        {
            wasPinching = isPinching;
        }
    }

    Vector3 GetIndexTipWorldPos()
    {
        var skel = ovrHand.GetComponent<OVRSkeleton>();
        foreach (var bone in skel.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip)
                return bone.Transform.position;
        }
        return Vector3.zero;
    }

    void TriggerUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
            Debug.Log("Index slide gesture triggered UI.");
        }
    }
    
    private bool IsOneGesture(OVRHand hand)
    {
        Debug.Log("IsOneGesture Index: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Index));
        Debug.Log("IsOneGesture Thumb: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb));
        Debug.Log("IsOneGesture Middle: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle));
        Debug.Log("IsOneGesture Ring: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring));
        Debug.Log("IsOneGesture Pinky: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky));            

        // 食指未捏合（= 伸出）
        bool indexExtended = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < 0.3f;

        // 其他手指都在“捏合状态”或弯曲（= 收起）
        bool thumbClosed  = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) > 0.7f;
        bool middleClosed = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.7f;
        bool ringClosed   = hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > 0.5f;

        return indexExtended && middleClosed && ringClosed;
    }

}
