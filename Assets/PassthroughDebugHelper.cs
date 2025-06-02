// using UnityEngine;
// using UnityEngine.XR.OpenXR;
// using UnityEngine.XR.OpenXR.Features.Meta;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif

// public class PassthroughDebugHelper : MonoBehaviour
// {
//     void Start()
//     {
//         Debug.Log("=== [Passthrough Debug Helper] ===");

//         // 检查 OpenXR 是否启用
//         if (!OpenXRRuntime.isStarted)
//         {
//             Debug.LogWarning("OpenXR runtime has not started.");
//             return;
//         }

//         // 检查 MetaPassthroughFeature 是否启用
//         var metaPassthroughFeature = OpenXRSettings.Instance.GetFeature<MetaPassthroughFeature>();
//         if (metaPassthroughFeature == null || !metaPassthroughFeature.enabled)
//         {
//             Debug.LogWarning("MetaPassthroughFeature is NOT enabled in OpenXR settings.");
//         }
//         else
//         {
//             Debug.Log("MetaPassthroughFeature is enabled.");
//         }

//         // 检查 MetaEnvironmentDepthFeature
//         var depthFeature = OpenXRSettings.Instance.GetFeature<MetaEnvironmentDepthFeature>();
//         if (depthFeature == null || !depthFeature.enabled)
//         {
//             Debug.LogWarning("MetaEnvironmentDepthFeature is NOT enabled in OpenXR settings.");
//         }
//         else
//         {
//             Debug.Log("MetaEnvironmentDepthFeature is enabled.");
//         }

//         // 检查环境深度支持
//         Debug.Log($"EnvironmentDepthManager.IsSupported: {Meta.XR.EnvironmentDepth.EnvironmentDepthManager.IsSupported}");

//         // 尝试自动添加 PassthroughLayer（仅在运行时）
//         TryEnablePassthroughLayer();
//     }

//     private void TryEnablePassthroughLayer()
//     {
//         var existing = FindObjectOfType<UnityEngine.XR.ARSubsystems.XRPassthroughSubsystem>();
//         if (existing != null && !existing.running)
//         {
//             Debug.Log("Found XRPassthroughSubsystem, attempting to start...");
//             existing.Start();
//         }
//         else if (existing == null)
//         {
//             Debug.LogWarning("XRPassthroughSubsystem not found in scene.");
//         }
//         else
//         {
//             Debug.Log("XRPassthroughSubsystem is already running.");
//         }
//     }
// }
