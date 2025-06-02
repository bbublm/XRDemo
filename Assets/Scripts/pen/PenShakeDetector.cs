using UnityEngine;

public class PenShakeDetector : MonoBehaviour
{
    public GameObject recordingUI;

    private Vector3 lastPos;
    private float shakeThreshold = 1.5f;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        float speed = (transform.position - lastPos).magnitude / Time.deltaTime;
        if (speed > shakeThreshold)
        {
            recordingUI.SetActive(true);
        }
        lastPos = transform.position;
    }
}
