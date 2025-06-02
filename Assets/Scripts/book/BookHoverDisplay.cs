using UnityEngine;

public class BookHoverDisplay : MonoBehaviour
{
    public GameObject infoPanel;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Hand"))
        {
            Vector3 directionToBook = (transform.position - other.transform.position).normalized;
            float angle = Vector3.Angle(other.transform.forward, directionToBook);

            if (angle < 30f)
            {
                infoPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hand"))
        {
            infoPanel.SetActive(false);
        }
    }
}
