using UnityEngine;

public class ShakeAndGestureTrigger : MonoBehaviour
{
    public OVRHand ovrHand; // 左或右手
    public float pinchThreshold = 0.9f;  // 捏合阈值
    public float directionChangeThreshold = -0.5f; // Dot product 小于此值说明方向反转
    public float requiredShakeCount = 3;  // 至少多少次来回反转
    public float shakeWindowTime = 1.0f;  // 在这个时间内完成

    private Vector3 lastPos;
    private Vector3 lastVelocity;

    private int directionChangeCount = 0;
    private float shakeTimer = 0f;
    public GameObject uiPanel;

    void Start()
    {
        lastPos = ovrHand.transform.position;
    }

    void Update()
    {
        if (ovrHand == null || !ovrHand.IsTracked) return;

        // 1. 判断捏合状态
        bool isPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold &&
                          ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) > pinchThreshold;

        Debug.Log("isPinching: " + isPinching);
        // 2. 计算速度方向
        Vector3 currentPos = ovrHand.transform.position;
        Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;

        // 3. 判断方向是否反转（dot product 小于负阈值）
        float dot = Vector3.Dot(velocity.normalized, lastVelocity.normalized);
        Debug.Log("dot: " + dot);
        if (dot < directionChangeThreshold && velocity.magnitude > 0.1f) // 加上速度门槛避免静止时误触
        {
            directionChangeCount++;
            Debug.Log("Direction reversed! Count: " + directionChangeCount);
        }

        // 4. 时间统计
        shakeTimer += Time.deltaTime;
        if (shakeTimer > shakeWindowTime)
        {
            directionChangeCount = 0;
            shakeTimer = 0f;
        }

        // 5. 触发 UI
        if (directionChangeCount >= requiredShakeCount)
        {
            TriggerUI();
            ResetShakeState(); // 重置防止连触发
        }

        lastVelocity = velocity;
        lastPos = currentPos;
    }

    void TriggerUI()
    {
        if (uiPanel != null)
        {
            Debug.Log("trigger ui");
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }

    void ResetShakeState()
    {
        directionChangeCount = 0;
        shakeTimer = 0f;
        lastVelocity = Vector3.zero;
    }
}
