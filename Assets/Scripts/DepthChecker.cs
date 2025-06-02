// using UnityEngine;

// public class DepthChecker : MonoBehaviour
// {
//     public Camera xrCamera;
//     public bool useLinearDepth = true;
//     public float logInterval = 1f;
//     public bool logDetailedInfo = true;

//     public Material depthSampleMaterial;

//     private float timer = 0f;

//     private static readonly int DepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

//     private void Start(){
//         xrCamera.depthTextureMode |= DepthTextureMode.Depth;
//     }

//     private void Update()
//     {
//         timer += Time.deltaTime;
//         if (timer >= logInterval)
//         {
//             timer = 0f;
//             LogDepthInfo();
//         }
//     }

//     private void LogDepthInfo()
//     {
//         // Texture2D depthTex = OVRManager.instance.GetRawDepthTexture();

//         var texPtr = OVRPlugin.GetDepthTexture();


//         Texture depthTexture = Shader.GetGlobalTexture(DepthTextureID);
//         if (depthTexture == null)
//         {
//             Debug.LogWarning("Depth texture not available.");
//             return;
//         }

//         Vector2 centerUV = new Vector2(0.5f, 0.5f);
//         float rawDepth = GetDepthAtUV(depthTexture, centerUV);

//         if (rawDepth <= 0f || rawDepth >= 1f)
//         {
//             Debug.Log("Center depth value invalid or out of range.");
//             return;
//         }

//         float actualDistance = useLinearDepth
//             ? LinearizeDepth(rawDepth, xrCamera.nearClipPlane, xrCamera.farClipPlane)
//             : rawDepth;

//         string logMessage = $"Center Depth Info:\n" +
//                             $"Raw Depth: {rawDepth:F3}\n" +
//                             $"Actual Distance: {actualDistance:F2}m";

//         if (logDetailedInfo)
//         {
//             logMessage += $"\nCamera Near: {xrCamera.nearClipPlane:F2}m" +
//                           $"\nCamera Far: {xrCamera.farClipPlane:F2}m" +
//                           $"\nDepth Texture Size: {depthTexture.width}x{depthTexture.height}";
//         }

//         Debug.Log(logMessage);
//     }

//     private float GetDepthAtUV(Texture depthTexture, Vector2 uv)
//     {
//         RenderTexture rt = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
//         rt.Create();

//         try
//         {
//             depthSampleMaterial.SetTexture("_MainTex", depthTexture);
//             depthSampleMaterial.SetVector("_UV", new Vector4(uv.x, uv.y, 0, 0));

//             Graphics.Blit(null, rt, depthSampleMaterial);

//             RenderTexture.active = rt;
//             Texture2D tex = new Texture2D(1, 1, TextureFormat.RFloat, false);
//             tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
//             tex.Apply();
//             RenderTexture.active = null;

//             return tex.GetPixel(0, 0).r;
//         }
//         finally
//         {
//             rt.Release();
//             Destroy(rt);
//         }
//     }

//     private float LinearizeDepth(float ndcDepth, float nearZ, float farZ)
//     {
//         float z = ndcDepth * 2f - 1f;
//         return (2f * nearZ * farZ) / (farZ + nearZ - z * (farZ - nearZ));
//     }
// }
