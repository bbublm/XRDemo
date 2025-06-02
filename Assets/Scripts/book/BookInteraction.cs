using UnityEngine;

public class BookInteraction : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 1.5f;
    public Material highlightMat;
    public Material defaultMat;

    private Renderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < triggerDistance)
        {
            meshRenderer.material = highlightMat;
        }
        else
        {
            meshRenderer.material = defaultMat;
        }
    }
}
