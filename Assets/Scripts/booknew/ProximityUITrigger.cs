using UnityEngine;

public class ProximityUITrigger : MonoBehaviour
{
    public Transform playerHead;     // XR Rig 中的 Camera 或 CenterEyeAnchor
    public GameObject uiPanel;       // 要显示或隐藏的 UI Panel
    public float triggerDistance = 0.4f; // 触发距离（单位：米）

    void Update()
    {
        Debug.Log("playerHead" + playerHead);
        Debug.Log("uiPanel" + uiPanel);
        if (playerHead == null || uiPanel == null)
            return;

        float distance = Vector3.Distance(uiPanel.transform.position, playerHead.position);

        Debug.Log("distance: " + distance);
        if (distance < triggerDistance)
        {
            if (!uiPanel.activeSelf)
                uiPanel.SetActive(true);
        }
        else
        {
            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);
        }
    }
}
