using UnityEngine;
using Meta.XR.EnvironmentDepth;  // ✅ 正确命名空间

public static class EnvironmentDepthExtensions
{
    /// <summary>
    /// 获取视口坐标对应的环境深度（线性化前的 raw NDC 值，0~1）
    /// </summary>
    public static float GetRawEnvironmentDepth(this EnvironmentDepthManager manager, Vector2 viewportUV)
    {
        var rt = Shader.GetGlobalTexture("_EnvironmentDepthTexture") as RenderTexture;
        if (rt == null) return -1f;

        int x = Mathf.Clamp((int)(viewportUV.x * rt.width), 0, rt.width - 1);
        int y = Mathf.Clamp((int)(viewportUV.y * rt.height), 0, rt.height - 1);

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(1, 1, TextureFormat.R16, false, true);
        tex.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Color pixel = tex.GetPixel(0, 0);
        UnityEngine.Object.Destroy(tex);

        return pixel.r; // Depth in NDC space [0,1]
    }
}
