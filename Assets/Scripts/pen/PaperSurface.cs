using UnityEngine;

public class PaperSurface : MonoBehaviour
{
    private void Awake()
    {
        gameObject.tag = "PaperSurface";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}

