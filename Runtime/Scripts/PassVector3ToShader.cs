using UnityEngine;

[ExecuteAlways]
public class PassVector3ToShader : MonoBehaviour
{
    [Tooltip("The target GameObject whose world position will be passed to the shader.")]
    public GameObject targetObject;

    [Tooltip("The GameObject with the MeshRenderer whose materials will receive the vector.")]
    public GameObject meshRendererObject;

    [Tooltip("The name of the Vector3 parameter in the shader.")]
    public string shaderParameterName = "_TargetPosition";

    private MeshRenderer meshRenderer;

    void OnEnable()
    {
        if (meshRendererObject != null)
            meshRenderer = meshRendererObject.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (targetObject == null || meshRenderer == null || string.IsNullOrEmpty(shaderParameterName))
            return;

        Vector3 worldPos = targetObject.transform.position;

        // Update all materials on the MeshRenderer
        foreach (var mat in meshRenderer.materials)
        {
            if (mat != null)
                mat.SetVector(shaderParameterName, worldPos);
        }
    }
}