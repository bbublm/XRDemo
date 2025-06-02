using UnityEngine;

public class RotationTriggerUI : MonoBehaviour
{
    // public GameObject uiPanel;            // 要显示/隐藏的 UI 面板
    public float angleThreshold = 160f;   // 判断是否接近 180 度的角度容差
    public bool checkX = true;            // 是否检测绕 X 轴
    public bool checkY = true;            // 是否检测绕 Y 轴

    private Quaternion initialRotation;
    private bool hasTriggered = false;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        Quaternion currentRotation = transform.rotation;
        Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * currentRotation;
        Vector3 deltaEuler = deltaRotation.eulerAngles;

        float xAngle = Mathf.DeltaAngle(0, deltaEuler.x);
        float yAngle = Mathf.DeltaAngle(0, deltaEuler.y);

        bool xFlipped = checkX && Mathf.Abs(xAngle) > angleThreshold;
        bool yFlipped = checkY && Mathf.Abs(yAngle) > angleThreshold;

        if ((xFlipped || yFlipped) && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("🔄 旋转触发 UI");
        }

        // 可选：旋转回原角度后允许再次触发
        if (!xFlipped && !yFlipped)
        {
            hasTriggered = false;
        }
    }
}
