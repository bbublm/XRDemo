using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class CapDetachOnGrab : MonoBehaviour
{
    public Transform pen;                      // 引用笔的位置
    public float reattachDistance = 0.05f;     // 回吸距离
    public bool enableAutoReattach = true;     // 是否启用自动吸附

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isDetached = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!isDetached)
        {
            // 分离笔帽
            transform.parent = null;
            rb.isKinematic = false;
            isDetached = true;
        }
    }

    void Update()
    {
        if (enableAutoReattach && isDetached && !grabInteractable.isSelected)
        {
            float dist = Vector3.Distance(transform.position, pen.position);
            if (dist < reattachDistance)
            {
                ReattachCap();
            }
        }
    }

    private void ReattachCap()
    {
        transform.SetParent(pen);
        transform.localPosition = Vector3.zero;    // 可根据需要微调位置
        transform.localRotation = Quaternion.identity;

        rb.isKinematic = true;
        isDetached = false;
    }
}
