// using UnityEngine;
// using Oculus.Interaction;
// using Oculus.Interaction.OVR.Input;
// using Oculus.Platform;
// using Oculus.Platform.Models;
// using Oculus.SpatialAnchors;  // <-- 用于锚点管理
// using UnityEngine.XR;
// using System.Collections.Generic;
// using UnityEngine.XR.Management;

// public class AnchorManager : MonoBehaviour
// {
//     public GameObject objectPrefab; // 要放置的虚拟物体
//     public Camera xrCamera;         // XR主摄像头

//     private void Update()
//     {
//         if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
//         {
//             Vector2 screenPos = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

//             // 将点击位置映射为射线
//             Ray ray = xrCamera.ScreenPointToRay(screenPos);
//             if (Physics.Raycast(ray, out RaycastHit hit, 10f))
//             {
//                 CreateAnchorAtPosition(hit.point);
//             }
//             else
//             {
//                 // 没有碰撞，默认 2m 前方
//                 Vector3 fallback = ray.origin + ray.direction * 2f;
//                 CreateAnchorAtPosition(fallback);
//             }
//         }
//     }

//     private void CreateAnchorAtPosition(Vector3 position)
//     {
//         GameObject anchorGO = new GameObject("Anchor_" + System.Guid.NewGuid());
//         anchorGO.transform.position = position;
//         anchorGO.transform.rotation = Quaternion.identity;

//         // 添加 Meta Anchor 组件
//         var anchorComponent = anchorGO.AddComponent<Oculus.XR.Anchor>();
//         anchorComponent.SaveAnchorAsync().ContinueWith((task) =>
//         {
//             if (task.Result)
//             {
//                 Debug.Log("✅ Anchor saved successfully");
//                 Instantiate(objectPrefab, position, Quaternion.identity, anchorGO.transform);
//             }
//             else
//             {
//                 Debug.LogWarning("❌ Anchor save failed");
//             }
//         });
//     }
// }
