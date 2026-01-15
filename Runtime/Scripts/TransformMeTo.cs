using UnityEngine;

public class TransformMeTo : MonoBehaviour
{
    [Tooltip("The target GameObject whose transform will be copied to this object.")]
    public GameObject target;

    [Header("Rotation Offsets (degrees)")]
    public Vector3 rotationOffsetEuler = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("TransformMeTo: Target GameObject is not assigned.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Set position directly to target's position
        transform.position = target.transform.position;

        // Set rotation with offset
        Quaternion baseRotation = target.transform.rotation;
        Quaternion offsetRotation = Quaternion.Euler(rotationOffsetEuler);
        transform.rotation = baseRotation * offsetRotation;

        // Do NOT change scale - leave self's scale as is
    }
}