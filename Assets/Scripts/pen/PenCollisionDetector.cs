using UnityEngine;

public class PenCollisionDetector : MonoBehaviour
{
    public Canvas uiPanel;  // 指向 UI 面板
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PaperSurface"))
        {
            Debug.Log("Pen touched paper!");
            ShowPanel();
        }
    }

    void ShowPanel()
    {
        if (uiPanel != null)
            uiPanel.gameObject.SetActive(true);
    }
}
