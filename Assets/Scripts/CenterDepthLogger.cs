using UnityEngine;
using Meta.XR.EnvironmentDepth;
using System.Collections;

public class CenterDepthLogger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float logInterval = 1.0f;
    [SerializeField] private bool useLinearDepth = true;
    [SerializeField] private bool logDetailedInfo = true;

    [Header("References")]
    [SerializeField] private Camera xrCamera;
    [SerializeField] private EnvironmentDepthManager depthManager;

    private float timer = 0f;
    private bool isInitialized = false;
    private static readonly int DepthTextureID = Shader.PropertyToID("_EnvironmentDepthTexture");
    private static readonly int ZBufferParamsID = Shader.PropertyToID("_EnvironmentDepthZBufferParams");

    private RenderTexture _depthTexture;
    private Vector4 _environmentDepthZBufferParams;
    [SerializeField] public Texture2D _readableDepthTexture;

    private float nearClipPlane = 0.01f; // 替换为你的相机的近平面距离
    private float farClipPlane = 100.0f; // 替换为你的相机的远平面距离

    // 深度值类型
    public enum DepthType
    {
        Raw,        // 原始深度值 [0,1]
        Linear,     // 线性深度值（米）
        WorldSpace  // 世界空间深度值（米）
    }

    [SerializeField] private DepthType depthType = DepthType.Linear;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (xrCamera == null)
        {
            xrCamera = Camera.main;
            if (xrCamera == null)
            {
                Debug.LogError("No camera found! Please assign a camera reference.");
                enabled = false;
                return;
            }
        }

        // 检查深度API支持
        if (!EnvironmentDepthManager.IsSupported)
        {
            Debug.LogError("Environment Depth is not supported on this device!");
            enabled = false;
            return;
        }

        // 获取深度管理器
        depthManager = FindObjectOfType<EnvironmentDepthManager>();
        if (depthManager == null)
        {
            Debug.LogError("EnvironmentDepthManager not found in scene!");
            enabled = false;
            return;
        }

        // 配置深度管理器
        depthManager.enabled = true;
        depthManager.OcclusionShadersMode = OcclusionShadersMode.SoftOcclusion;

        // 等待深度API可用
        StartCoroutine(WaitForDepthAvailability());
    }

    private IEnumerator WaitForDepthAvailability()
    {
        while (!depthManager.IsDepthAvailable)
        {
            Debug.Log("Waiting for depth API to become available...");
            yield return new WaitForSeconds(0.5f);
        }

        isInitialized = true;
        Debug.Log("Depth API is now available!");
    }

    private void Update()
    {
        if (!isInitialized || !depthManager.IsDepthAvailable)
        {
            Debug.Log("Depth API is not available or not initialized.");
            return;
        }
        Debug.Log("update 1");

        timer += Time.deltaTime;
        if (timer < logInterval)
        {
            Debug.Log("Waiting for next log interval...");
            return;
        }

        Debug.Log("update 2");

        LogCenterDepth();
        timer = 0f;
    }

    private void LogCenterDepth()
    {
        try
        {
            Debug.Log("LogCenterDepth");

            nearClipPlane = xrCamera.nearClipPlane;
            farClipPlane = xrCamera.farClipPlane;
            Debug.Log($"Camera Position: {xrCamera.transform.position}");
            Debug.Log($"Camera Rotation: {xrCamera.transform.rotation.eulerAngles}");
            
            Debug.Log($"nearClipPlane: {xrCamera.nearClipPlane}");
            Debug.Log($"farClipPlane: {xrCamera.farClipPlane}");

            _depthTexture = Shader.GetGlobalTexture("_EnvironmentDepthTexture") as RenderTexture;
            Debug.Log($"Depth Texture Instance ID: {_depthTexture.GetInstanceID()}");

            Debug.Log($"Depth texture format: {_depthTexture.graphicsFormat}");
            // _environmentDepthZBufferParams = Shader.GetGlobalVector("_EnvironmentDepthZBufferParams");
            _environmentDepthZBufferParams = EnvironmentDepthUtils.ComputeNdcToLinearDepthParameters(nearClipPlane, farClipPlane);

            Debug.Log($"Calculated ZBufferParams: {_environmentDepthZBufferParams}");


            if (_depthTexture != null)
            {
                // 如果还没有创建可读纹理，或者尺寸不匹配，则创建
                if (_readableDepthTexture == null || _readableDepthTexture.width != _depthTexture.width || _readableDepthTexture.height != _depthTexture.height)
                {
                    if (_readableDepthTexture != null)
                    {
                        Destroy(_readableDepthTexture);
                    }

                    // 使用与深度纹理相同的格式
                    _readableDepthTexture = new Texture2D(_depthTexture.width, _depthTexture.height, TextureFormat.R16, false); // 使用TextureFormat.R16来匹配RenderTexture的GraphicsFormat.R16_UNorm

                }
                // 保存当前的活动渲染纹理
                RenderTexture currentRT = RenderTexture.active;

                // 设置活动渲染纹理为深度纹理
                RenderTexture.active = _depthTexture;

                // 将渲染纹理的数据读取到 Texture2D
                _readableDepthTexture.ReadPixels(new Rect(0, 0, _depthTexture.width, _depthTexture.height), 0, 0);
                _readableDepthTexture.Apply();

                float minDepth = float.MaxValue;
                float maxDepth = float.MinValue;
                float sumDepth = 0;

                for (int x = 0; x < _depthTexture.width; x++)
                {
                    for (int y = 0; y < _depthTexture.height; y++)
                    {
                        float depth = _readableDepthTexture.GetPixel(x, y).r;
                        if (depth < minDepth) minDepth = depth;
                        if (depth > maxDepth) maxDepth = depth;
                        sumDepth += depth;
                    }
                }

                float averageDepth = sumDepth / (_depthTexture.width * _depthTexture.height);
                // 计算标准差（需要先计算平均值）

                Debug.Log($"Min Depth: {minDepth}, Max Depth: {maxDepth}, Average Depth: {averageDepth}");

                Color centerColor = _readableDepthTexture.GetPixel(_readableDepthTexture.width / 2, _readableDepthTexture.height / 2);
                Debug.Log($"Center pixel color: {centerColor}");
                // 恢复原来的活动渲染纹理
                RenderTexture.active = currentRT;

                // 获取中心点的深度值
                int centerX = _depthTexture.width / 2;
                int centerY = _depthTexture.height / 2;

                Debug.Log("centerX: " + centerX);
                Debug.Log("centerY: " + centerY);

                float depthValue = _readableDepthTexture.GetPixel(centerX, centerY).r;

                // Debug logs for troubleshooting
                Debug.Log($"Depth texture size: {_depthTexture.width}x{_depthTexture.height}");
                Debug.Log($"ZBufferParams: {_environmentDepthZBufferParams}");
                Debug.Log($"Normalized depth value: {depthValue}");

                //获取ZBuffer参数
                // var leftEyeData = EnvironmentDepthManager.GetDepthFrameDesc(0);
                // _environmentDepthZBufferParams = EnvironmentDepthUtils.ComputeNdcToLinearDepthParameters(leftEyeData.nearZ, leftEyeData.farZ);
                // Debug.Log($"Calculated ZBufferParams: {_environmentDepthZBufferParams}");

                // 计算线性深度
                float linearDepth = 1.0f / (_environmentDepthZBufferParams.x * depthValue + _environmentDepthZBufferParams.y);

                Debug.Log("Center Linear Depth: " + linearDepth);
            }
            else
            {
                Debug.Log("No Depth Texture Available");
            }
            
            // // 获取深度纹理
            // Texture depthTexture = Shader.GetGlobalTexture(DepthTextureID);
            // if (depthTexture == null)
            // {
            //     Debug.LogWarning("Depth texture is not available");
            //     return;
            // }
            // Debug.Log("depthTexture" + depthTexture);

            // // 获取中心点深度
            // Vector2 centerUV = new Vector2(0.5f, 0.5f);
            // float rawDepth = GetDepthAtUV(depthTexture, centerUV);

            // if (rawDepth <= 0f || rawDepth >= 1f)
            // {
            //     Debug.Log("Center depth value invalid or out of range.");
            //     return;
            // }

            // // 获取深度参数
            // Vector4 zBufferParams = Shader.GetGlobalVector(ZBufferParamsID);
            
            // // 根据选择的深度类型计算深度值
            // float finalDepth = GetDepthValue(rawDepth, zBufferParams);

            // // 构建日志信息
            // string logMessage = $"Center Depth Info:\n" +
            //                   $"Raw Depth: {rawDepth:F3}\n" +
            //                   $"Final Depth: {finalDepth:F2}m\n" +
            //                   $"Depth Type: {depthType}";

            // if (logDetailedInfo)
            // {
            //     logMessage += $"\nZBuffer Params: {zBufferParams}\n" +
            //                 $"\nDepth Texture Size: {depthTexture.width}x{depthTexture.height}";
            // }

            // Debug.Log(logMessage);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting depth info: {e.Message}");
        }
    }

    private float GetDepthValue(float rawDepth, Vector4 zBufferParams)
    {
        switch (depthType)
        {
            case DepthType.Raw:
                return rawDepth;
            
            case DepthType.Linear:
                return LinearizeDepth(rawDepth, zBufferParams);
            
            case DepthType.WorldSpace:
                float linearDepth = LinearizeDepth(rawDepth, zBufferParams);
                return ConvertToWorldSpace(linearDepth);
            
            default:
                return rawDepth;
        }
    }

    private float GetDepthAtUV(Texture depthTexture, Vector2 uv)
    {
        // 创建临时RenderTexture
        RenderTexture rt = new RenderTexture(depthTexture.width, depthTexture.height, 0, RenderTextureFormat.RFloat);
        rt.Create();

        try
        {
            // 复制深度纹理
            Graphics.Blit(depthTexture, rt);
            
            // 读取深度值
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RFloat, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            // 计算UV坐标对应的像素位置
            int x = Mathf.Clamp((int)(uv.x * tex.width), 0, tex.width - 1);
            int y = Mathf.Clamp((int)(uv.y * tex.height), 0, tex.height - 1);

            // 获取深度值
            float depth = tex.GetPixel(x, y).r;
            
            // 清理资源
            Destroy(tex);
            return depth;
        }
        finally
        {
            if (rt != null)
            {
                rt.Release();
                Destroy(rt);
            }
        }
    }

    private float LinearizeDepth(float depth, Vector4 zBufferParams)
    {
        // 使用ZBuffer参数进行线性化
        return 1.0f / (zBufferParams.z * depth + zBufferParams.w);
    }

    private float ConvertToWorldSpace(float linearDepth)
    {
        // 将线性深度转换为世界空间距离
        // 这里需要根据相机的参数进行转换
        if (xrCamera != null)
        {
            // 使用相机的近平面和远平面进行转换
            float near = xrCamera.nearClipPlane;
            float far = xrCamera.farClipPlane;
            return Mathf.Lerp(near, far, linearDepth);
        }
        return linearDepth;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}