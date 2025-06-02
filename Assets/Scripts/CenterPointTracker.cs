using UnityEngine;
using UnityEngine.UI;
using Meta.XR.EnvironmentDepth;  // ✅ 正确命名空间

public class CenterPointTracker : MonoBehaviour
{
    public Camera xrCamera;
    public GameObject marker;
    public Text coordText;

    public float nearZ = 0.01f;
    public float farZ = 5f;
    public float depthSmoothing = 0.8f;

    private EnvironmentDepthManager depthManager;
    private float smoothedDepth = 0f;

    void Start()
    {
        depthManager = FindObjectOfType<EnvironmentDepthManager>();
        if (depthManager == null)
        {
            Debug.LogError("EnvironmentDepthManager not found.");
            enabled = false;
        }
    }

    void Update()
    {
        if (depthManager == null || !depthManager.IsDepthAvailable) return;

        Vector2 center = new Vector2(0.5f, 0.5f);
        float rawDepth = depthManager.GetRawEnvironmentDepth(center);

        if (rawDepth <= 0f || rawDepth >= 1f) return;

        float linearDepth = LinearizeDepth(rawDepth, nearZ, farZ);

        if (linearDepth < nearZ || linearDepth > farZ)
            return;

        smoothedDepth = Mathf.Lerp(smoothedDepth, linearDepth, 1f - depthSmoothing);
        Vector3 viewPos = new Vector3(center.x, center.y, smoothedDepth);
        Vector3 worldPos = xrCamera.ViewportToWorldPoint(viewPos);

        marker.transform.position = worldPos;

        if (coordText != null)
            coordText.text = $"World Pos: {worldPos:F3}m\nDepth: {smoothedDepth:F2}m";
    }

    float LinearizeDepth(float ndcDepth, float nearZ = 0.01f, float farZ = 5f)
    {
        float z = ndcDepth * 2f - 1f; // Convert [0,1] to [-1,1]
        return (2f * nearZ * farZ) / (farZ + nearZ - z * (farZ - nearZ));
    }
}