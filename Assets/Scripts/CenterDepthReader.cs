// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.XR;
// using UnityEngine.XR.Management;
// using Meta.XR.Depth;

// public class CenterDepthReader : MonoBehaviour
// {
//     public Camera targetCamera;
//     public RenderTexture debugDepthTexture;

//     void Start()
//     {
//         if (targetCamera == null)
//             targetCamera = Camera.main;
//     }

//     void Update()
//     {
//         RenderTexture envDepth = EnvironmentDepthTextureProvider.GetEnvironmentDepthTexture();

//         if (envDepth == null || !envDepth.IsCreated())
//         {
//             Debug.LogWarning("Environment depth texture not available.");
//             return;
//         }

//         // 1. 中心点UV
//         Vector2 centerUV = new Vector2(0.5f, 0.5f);

//         // 2. 将 depthTexture 拷贝到 CPU 可读取的临时纹理
//         RenderTexture.active = envDepth;
//         Texture2D readTex = new Texture2D(envDepth.width, envDepth.height, TextureFormat.RFloat, false);
//         readTex.ReadPixels(new Rect(0, 0, envDepth.width, envDepth.height), 0, 0);
//         readTex.Apply();

//         // 3. 读取深度值
//         Color pixel = readTex.GetPixelBilinear(centerUV.x, centerUV.y);
//         float linearDepth = pixel.r;

//         // 4. 计算世界坐标（从视图坐标还原）
//         Vector3 viewportPos = new Vector3(centerUV.x, centerUV.y, linearDepth);
//         Vector3 worldPos = targetCamera.ViewportToWorldPoint(viewportPos);

//         Debug.Log($"Center depth: {linearDepth} meters, world position: {worldPos}");

//         Destroy(readTex); // 清理
//     }
// }
