using UnityEngine;

public class BookDoubleTap : MonoBehaviour
{
    public GameObject recommendPanel;
    private float lastTapTime = 0f;

    void Update()
    {
        // 以右手触控按钮模拟双击
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            if (Time.time - lastTapTime < 0.4f)
            {
                recommendPanel.SetActive(true);
                lastTapTime = 0f;
            }
            else
            {
                lastTapTime = Time.time;
            }
        }
    }
}
