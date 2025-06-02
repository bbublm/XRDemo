using UnityEngine;

public class OpenHandGestureTrigger : MonoBehaviour
{
    public OVRHand ovrHand;
    public Transform uiPanel;
    public float pinchThreshold = 0.3f;
    public float requiredHoldTime = 0.4f;

    private float gestureTimer = 0f;
    public float proximityThreshold = 0.1f; // 5cm 距离判定

    private float elapsedTime = 0f;
    private bool delayPassed = false;

    void Update()
    {
        // 计时
        if (!delayPassed)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10f)
            {
                delayPassed = true;
            }
            else
            {
                return; // 等待 10 秒前，不执行后续逻辑
            }
        }

        if (ovrHand == null || !ovrHand.IsTracked) return;

        bool allFingersOpen = isHover(ovrHand);

        // 获取手的位置
        Vector3 handPos = ovrHand.transform.position;

        // 获取 cube 的左侧面世界坐标
        // Vector3 cubeLeftWorldPos = uiPanel.transform.position - uiPanel.transform.right * (uiPanel.transform.lossyScale.x / 2f);

        // 计算与左侧面的距离
        float distanceToLeft = Vector3.Distance(handPos, uiPanel.position);

        // Debug
        Debug.Log("🖐️ 手张开: " + allFingersOpen + "，距离 Cube 左侧: " + distanceToLeft);
        // Debug.Log("🖐️ 手张开: " + handPos + cubeLeftWorldPos + Color.red);

        if (allFingersOpen && distanceToLeft < proximityThreshold)
        {
            gestureTimer += Time.deltaTime;
            if (gestureTimer > requiredHoldTime)
            {
                Debug.Log("🖐️ 识别到张手手势，显示 UI");                
            }
        }
        else
        {
            gestureTimer = 0f;
        }
    }

    private bool isHover(OVRHand hand)
    {
        Debug.Log("IsOneGesture Index: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Index));
        Debug.Log("IsOneGesture Thumb: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb));
        Debug.Log("IsOneGesture Middle: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle));
        Debug.Log("IsOneGesture Ring: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring));
        Debug.Log("IsOneGesture Pinky: " + hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky));            

        // 食指未捏合（= 伸出）
        bool indexExtended = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < 0.1f;
        bool ThumbExtended = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) < 0.1f;
        bool MiddleExtended = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) < 0.1f;
        bool RingExtended = hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) < 0.1f;
        bool PinkyExtended = hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) < 0.1f;


        return indexExtended && ThumbExtended && MiddleExtended && RingExtended && PinkyExtended;
    }
}
