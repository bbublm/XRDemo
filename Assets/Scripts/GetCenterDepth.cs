using Meta.XR.EnvironmentDepth;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class GetCenterDepth : MonoBehaviour
{
    [SerializeField] private Camera xrCamera;

    private RenderTexture _depthTexture;
    private RenderTexture _colorTexture;
    private Vector4 _environmentDepthZBufferParams;
    private Texture2D _readableDepthTexture;

    private float nearClipPlane = 0.01f;
    private float farClipPlane = 100.0f;

    void Start()
    {
        if (xrCamera == null)
        {
            Debug.LogError("XR camera not assigned! Please assign the XR camera in the Inspector.");
            return;
        }

        // 创建深度纹理
        _depthTexture = new RenderTexture(xrCamera.pixelWidth, xrCamera.pixelHeight, 16, GraphicsFormat.D16_UNorm);
        _depthTexture.name = "ManualDepthTexture";

        // 创建颜色纹理
        _colorTexture = new RenderTexture(xrCamera.pixelWidth, xrCamera.pixelHeight, 0, GraphicsFormat.R8G8B8A8_UNorm);
        _colorTexture.name = "ColorTexture";

        // 设置相机的渲染目标为颜色纹理
        xrCamera.targetTexture = _colorTexture;
    }

    void Update()
    {
        nearClipPlane = xrCamera.nearClipPlane;
        farClipPlane = xrCamera.farClipPlane;

        Debug.Log($"Near Plane: {nearClipPlane}, Far Plane: {farClipPlane}");

        if (_depthTexture == null)
        {
            Debug.LogError("Depth texture is null!");
            return;
        }

        _environmentDepthZBufferParams = EnvironmentDepthUtils.ComputeNdcToLinearDepthParameters(nearClipPlane, farClipPlane);

        Debug.Log($"Calculated ZBufferParams: {_environmentDepthZBufferParams}");

        if (_readableDepthTexture == null || _readableDepthTexture.width != _depthTexture.width || _readableDepthTexture.height != _depthTexture.height)
        {
            if (_readableDepthTexture != null)
            {
                Destroy(_readableDepthTexture);
            }
            _readableDepthTexture = new Texture2D(_depthTexture.width, _depthTexture.height, TextureFormat.R16, false);
        }

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = _depthTexture; // 尝试读取深度纹理，可能会有问题
        _readableDepthTexture.ReadPixels(new Rect(0, 0, _depthTexture.width, _depthTexture.height), 0, 0);
        _readableDepthTexture.Apply();
        RenderTexture.active = currentRT;

        int centerX = _depthTexture.width / 2;
        int centerY = _depthTexture.height / 2;

        float depthValue = _readableDepthTexture.GetPixel(centerX, centerY).r;

        Debug.Log($"Depth texture format: {_depthTexture.graphicsFormat}");
        Debug.Log($"Center pixel color: {_readableDepthTexture.GetPixel(centerX, centerY)}");

        float linearDepth = 1.0f / (_environmentDepthZBufferParams.x * depthValue + _environmentDepthZBufferParams.y);

        Debug.Log($"Normalized depth value: {depthValue}");
        Debug.Log("Center Linear Depth: " + linearDepth);
    }
}