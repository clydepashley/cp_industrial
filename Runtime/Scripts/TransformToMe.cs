using UnityEngine;

public class TransformToMe : MonoBehaviour
{
    [Tooltip("The target GameObject whose transform will be set to match this object's transform.")]
    public GameObject target;

    [Header("Rotation Offsets (degrees)")]
    public Vector3 rotationOffsetEuler = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("TransformToMe: Target GameObject is not assigned.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Set position directly
        target.transform.position = transform.position;

        // Set rotation with offset
        Quaternion baseRotation = transform.rotation;
        Quaternion offsetRotation = Quaternion.Euler(rotationOffsetEuler);
        target.transform.rotation = baseRotation * offsetRotation;

        // Do NOT change scale - leave target's scale as is
    }
}