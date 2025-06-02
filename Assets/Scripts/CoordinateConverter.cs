using UnityEngine;
using Meta.XR.BuildingBlocks;
using Meta.XR.EnvironmentDepth;
using System.Collections.Generic;

public class CoordinateConverter : MonoBehaviour
{
    [SerializeField] private GameObject anchorPrefab; // 锚点预制体
    [SerializeField] private Camera mainCamera; // 主相机
    [SerializeField] private EnvironmentDepthManager depthManager; // 深度管理器
    
    private SpatialAnchorCoreBuildingBlock spatialAnchorManager;
    private List<OVRSpatialAnchor> createdAnchors = new List<OVRSpatialAnchor>();
    private static readonly int DepthTextureID = Shader.PropertyToID("_EnvironmentDepthTexture");

    private void Start()
    {
        // 获取空间锚点管理器
        spatialAnchorManager = FindObjectOfType<SpatialAnchorCoreBuildingBlock>();
        if (spatialAnchorManager == null)
        {
            Debug.LogError("SpatialAnchorCoreBuildingBlock not found!");
            return;
        }

        // 确保深度管理器已启用
        if (depthManager != null)
        {
            depthManager.enabled = true;
            depthManager.OcclusionShadersMode = OcclusionShadersMode.SoftOcclusion;
        }
    }

    private void Update()
    {
        if (depthManager != null)
        {
            Debug.Log($"Depth Available: {depthManager.IsDepthAvailable}");
            Debug.Log($"Depth Texture: {Shader.GetGlobalTexture(DepthTextureID) != null}");
        }
    }

    // 将屏幕坐标转换为世界坐标
    public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        
        // 如果没有击中任何物体，使用深度API获取深度信息
        if (depthManager != null && depthManager.IsDepthAvailable)
        {
            // 获取深度纹理
            Texture depthTexture = Shader.GetGlobalTexture(DepthTextureID);
            if (depthTexture != null)
            {
                // 获取深度值
                float depth = GetDepthFromTexture(depthTexture, screenPoint);
                if (depth > 0)
                {
                    return mainCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, depth));
                }
            }
        }
        
        // 如果无法获取深度信息，返回一个默认距离
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 10f));
    }

    // 从深度纹理获取深度值
    private float GetDepthFromTexture(Texture depthTexture, Vector2 screenPoint)
    {
        // 将屏幕坐标转换为纹理坐标
        int x = Mathf.FloorToInt(screenPoint.x * depthTexture.width / Screen.width);
        int y = Mathf.FloorToInt(screenPoint.y * depthTexture.height / Screen.height);
        
        // 确保坐标在有效范围内
        x = Mathf.Clamp(x, 0, depthTexture.width - 1);
        y = Mathf.Clamp(y, 0, depthTexture.height - 1);
        
        // 创建临时RenderTexture来读取深度值
        RenderTexture rt = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
        rt.Create();
        
        // 使用Graphics.Blit将深度值复制到临时纹理
        Graphics.Blit(depthTexture, rt);
        
        // 读取深度值
        RenderTexture.active = rt;
        Texture2D tempTex = new Texture2D(1, 1, TextureFormat.RFloat, false);
        tempTex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
        tempTex.Apply();
        RenderTexture.active = null;
        
        // 获取深度值
        float depth = tempTex.GetPixel(0, 0).r;
        
        // 清理资源
        Destroy(rt);
        Destroy(tempTex);
        
        return depth;
    }

    // 在指定位置创建锚点
    public void CreateAnchorAtPosition(Vector3 worldPosition)
    {
        if (spatialAnchorManager != null && anchorPrefab != null)
        {
            // 创建锚点
            spatialAnchorManager.InstantiateSpatialAnchor(anchorPrefab, worldPosition, Quaternion.identity);
        }
    }

    // 清除所有锚点
    public void ClearAllAnchors()
    {
        if (spatialAnchorManager != null)
        {
            spatialAnchorManager.EraseAllAnchors();
        }
    }
} 