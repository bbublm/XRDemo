using UnityEngine;
using Meta.XR.EnvironmentDepth;
using UnityEngine.XR;
using Oculus.Interaction;

public class DepthClickToAnchor : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Camera mainCamera;
    public GameObject virtualPrefab;
    public EnvironmentDepthManager depthManager;

    private Texture2D depthTexture2D;

    void Update()
    {
        // 模拟中心点击（或手势、按钮）
        if (Input.GetMouseButtonDown(0) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            TryPlaceAnchorAtCenter();
        }
    }

    void TryPlaceAnchorAtCenter()
    {
        if (!depthManager.IsDepthAvailable)
        {
            Debug.LogWarning("Depth not available.");
            return;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray centerRay = mainCamera.ScreenPointToRay(screenCenter);

        if (TryGetWorldPositionFromDepth(centerRay, out Vector3 worldPos))
        {
            PlaceAnchor(worldPos);
        }
        else
        {
            Debug.LogWarning("未能从深度图中获得有效世界坐标");
        }
    }

    void PlaceAnchor(Vector3 worldPos)
    {
        GameObject anchorGO = new GameObject("Anchor_" + Time.time);
        anchorGO.transform.position = worldPos;

        var anchor = anchorGO.AddComponent<OVRSpatialAnchor>();
        anchor.Save((savedAnchor, success) =>
        {
            if (success)
            {
                Instantiate(virtualPrefab, anchorGO.transform);
                Debug.Log("锚点创建成功，虚拟物体已附着");
            }
            else
            {
                Debug.LogWarning("锚点保存失败");
            }
        });
    }

    /// <summary>
    /// 从深度纹理中采样并估算中心视线的世界坐标
    /// </summary>
    bool TryGetWorldPositionFromDepth(Ray ray, out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        // 从 Shader 全局获取深度纹理
        RenderTexture rt = Shader.GetGlobalTexture("_EnvironmentDepthTexture") as RenderTexture;
        if (rt == null) return false;

        // 拷贝为可读纹理
        if (depthTexture2D == null || depthTexture2D.width != rt.width || depthTexture2D.height != rt.height)
        {
            depthTexture2D = new Texture2D(rt.width, rt.height, TextureFormat.R16, false);
        }

        RenderTexture.active = rt;
        depthTexture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        depthTexture2D.Apply();
        RenderTexture.active = null;

        // 中心点像素位置
        int px = rt.width / 2;
        int py = rt.height / 2;

        float rawDepth = depthTexture2D.GetPixel(px, py).r;

        // 深度异常值过滤
        if (rawDepth <= 0.01f || rawDepth >= 5f)
        {
            return false;
        }

        // 将中心射线延伸到深度距离
        worldPosition = ray.origin + ray.direction * rawDepth;
        return true;
    }
}
