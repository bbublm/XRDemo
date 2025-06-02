using UnityEngine;

public class BookFlipComment : MonoBehaviour
{
    public GameObject commentBoard;

    void Update()
    {
        float flipAngle = Vector3.Angle(transform.up, Vector3.down);
        commentBoard.SetActive(flipAngle < 30f); // 检测是否翻转
    }
}
