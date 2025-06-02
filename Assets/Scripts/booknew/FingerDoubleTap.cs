using UnityEngine;

public class FingerDoubleTap : MonoBehaviour
{
    public OVRHand ovrHand;
    public OVRSkeleton skeleton;
    // public GameObject uiPanel;
    public Transform target;              // 被“点击”的物体，比如一个 Cube
    public float tapDistance = 0.03f;     // 指尖距离目标小于这个值算“点击”
    public float doubleTapMaxTime = 0.4f; // 两次点击之间最大间隔

    private float lastTapTime = -1f;
    private bool wasNearLastFrame = false;

    private Transform indexTip;

    void Start()
    {
        if (skeleton != null)
        {
            indexTip = FindBoneTransform(OVRSkeleton.BoneId.Hand_IndexTip);
        }
    }

    void Update()
    {
        if (ovrHand == null || !ovrHand.IsTracked || indexTip == null)
            return;

        float distance = Vector3.Distance(indexTip.position, target.position);
        bool isNear = distance < tapDistance;

        if (isNear && !wasNearLastFrame)
        {
            float now = Time.time;
            if (now - lastTapTime <= doubleTapMaxTime)
            {
                Debug.Log("✅ 双击触发！");
                // uiPanel.SetActive(!uiPanel.activeSelf);
                lastTapTime = -1f; // 重置
            }
            else
            {
                lastTapTime = now;
            }
        }

        wasNearLastFrame = isNear;
    }

    Transform FindBoneTransform(OVRSkeleton.BoneId boneId)
    {
        foreach (var bone in skeleton.Bones)
        {
            if (bone.Id == boneId)
                return bone.Transform;
        }
        return null;
    }
}
