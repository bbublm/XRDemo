using UnityEngine;
using UnityEngine.UI;

public class VisualizeDepthTexture : MonoBehaviour
{
    public RawImage rawImage; // 将 RawImage 组件拖放到这里
    public CenterDepthLogger centerDepthScript; // 将你的 GetCenterDepth 脚本拖放到这里

    void Update()
    {
        if (centerDepthScript == null)
        {
            Debug.LogError("CenterDepthScript not assigned! Please assign the CenterDepthScript in the Inspector.");
            return;
        }

        if (centerDepthScript._readableDepthTexture != null)
        {
            rawImage.texture = centerDepthScript._readableDepthTexture;
        }
        else
        {
            Debug.LogWarning("No depth texture available from CenterDepthScript.");
        }
    }
}