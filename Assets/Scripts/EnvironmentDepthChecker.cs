#if UNITY_ANDROID && UNITY_OPENXR
using UnityEngine;
using UnityEngine.XR;
using Meta.XR.EnvironmentDepth; // 确保你已引用 Meta XR SDK

[DefaultExecutionOrder(-100)]
public class EnvironmentDepthChecker : MonoBehaviour
{
    void Start()
    {
        Debug.Log("==== Meta XR Environment Depth 检测开始 ====");

        // 当前 XR 插件
        string loadedXRDevice = XRSettings.loadedDeviceName;
        Debug.Log($"[XR] 当前 XR 插件：{loadedXRDevice}");

        // 图形 API 检测
        Debug.Log($"[Graphics] 当前图形 API: {SystemInfo.graphicsDeviceType}");

        // 是否为支持 Meta XR 的设备
        string model = SystemInfo.deviceModel;
        Debug.Log($"[Device] 当前设备型号：{model}");

        // 检查 OpenXR 运行时是否为 Meta Quest
        bool isMetaRuntime = loadedXRDevice.ToLower().Contains("openxr");
        Debug.Log($"[XR] 是否为 OpenXR 模式: {isMetaRuntime}");

        // 检查环境深度支持
        bool depthSupported = EnvironmentDepthManager.IsSupported;
        Debug.Log($"[Depth] Environment Depth 是否支持: {depthSupported}");

        // 检查场景中是否已正确添加 EnvironmentDepthManager 脚本
        var depthMgr = FindObjectOfType<EnvironmentDepthManager>();
        if (depthMgr != null)
        {
            Debug.Log("[Scene] EnvironmentDepthManager 脚本已存在于场景中 ✅");
        }
        else
        {
            Debug.LogWarning("[Scene] ❌ EnvironmentDepthManager 脚本未添加到场景中！");
        }

        // 最终判定
        if (!depthSupported)
        {
            Debug.LogWarning("❌ 当前设备或配置不支持 Environment Depth，请检查 XR Plugin → OpenXR → Meta XR → Environment Depth 是否勾选！");
        }
        else
        {
            Debug.Log("✅ 当前环境支持 Environment Depth！");
        }

        Debug.Log("==== Meta XR Environment Depth 检测结束 ====");
    }
}
#endif
