using UnityEngine;

public class PenTouchPaper : MonoBehaviour
{
    public GameObject associatedUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paper"))
        {
            associatedUI.SetActive(true);
        }
    }
}
