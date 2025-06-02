using UnityEngine;

public class PenCapTrigger : MonoBehaviour
{
    public Transform pen;
    public Transform cap;
    public GameObject keywordBubbleUI;

    public float triggerDistance = 0.1f;

    void Update()
    {
        float distance = Vector3.Distance(pen.position, cap.position);
        if (distance > triggerDistance && !keywordBubbleUI.activeSelf)
        {
            keywordBubbleUI.SetActive(true);
        }
    }
}
