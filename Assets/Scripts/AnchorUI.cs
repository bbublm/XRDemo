using UnityEngine;
using UnityEngine.UI;

public class AnchorUI : MonoBehaviour
{
    [SerializeField] private Button placeAnchorButton;
    [SerializeField] private Button clearAnchorsButton;
    [SerializeField] private Text statusText;
    
    private void Start()
    {
        // 设置按钮文本
        if (placeAnchorButton != null)
        {
            placeAnchorButton.GetComponentInChildren<Text>().text = "放置锚点";
        }
        
        if (clearAnchorsButton != null)
        {
            clearAnchorsButton.GetComponentInChildren<Text>().text = "清除锚点";
        }
        
        // 设置状态文本
        if (statusText != null)
        {
            statusText.text = "准备就绪";
        }
    }
    
    public void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
} 