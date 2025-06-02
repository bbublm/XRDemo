using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnchorController : MonoBehaviour
{
    [SerializeField] private CoordinateConverter coordinateConverter;
    [SerializeField] private Button placeAnchorButton;
    [SerializeField] private Button clearAnchorsButton;
    
    private void Start()
    {
        // 设置按钮事件
        if (placeAnchorButton != null)
        {
            placeAnchorButton.onClick.AddListener(OnPlaceAnchorButtonClicked);
        }
        
        if (clearAnchorsButton != null)
        {
            clearAnchorsButton.onClick.AddListener(OnClearAnchorsButtonClicked);
        }
    }

    private void OnPlaceAnchorButtonClicked()
    {
        // 获取屏幕中心点
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        // 转换为世界坐标
        Vector3 worldPosition = coordinateConverter.ScreenToWorldPoint(screenCenter);
        
        // 创建锚点
        coordinateConverter.CreateAnchorAtPosition(worldPosition);
    }

    private void OnClearAnchorsButtonClicked()
    {
        coordinateConverter.ClearAllAnchors();
    }
} 