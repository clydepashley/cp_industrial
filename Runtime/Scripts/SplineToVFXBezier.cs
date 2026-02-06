using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineToVFXBezier : MonoBehaviour
{
    [SerializeField]
    SplineContainer splineContainer;

    [SerializeField]
    List<Vector3> knotPositions = new List<Vector3>();

    [SerializeField]
    List<Vector3> tangentInPositions = new List<Vector3>();

    [SerializeField]
    List<Vector3> tangentOutPositions = new List<Vector3>();

    [SerializeField]
    List<GameObject> knotObjects = new List<GameObject>();

    [SerializeField]
    List<GameObject> tangentInObjects = new List<GameObject>();

    [SerializeField]
    List<GameObject> tangentOutObjects = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (splineContainer == null)
        {
            return;
        }
        ReadSplinePositions();
        UpdateGameObjectPositions();
    }

    void ReadSplinePositions()
    {
        Spline spline = splineContainer.Spline;
        knotPositions.Clear();
        tangentInPositions.Clear();
        tangentOutPositions.Clear();

        foreach (var knot in spline.Knots)
        {
            Vector3 position = knot.Position;
            position += splineContainer.transform.position;
            Quaternion rotation = knot.Rotation;

            Vector3 tangentIn = position + (rotation * knot.TangentIn);
            Vector3 tangentOut = position + (rotation * knot.TangentOut);

            knotPositions.Add(position);
            tangentInPositions.Add(tangentIn);
            tangentOutPositions.Add(tangentOut);
        }
    }

    void UpdateGameObjectPositions()
    {
        for (int i = 0; i < knotObjects.Count && i < knotPositions.Count; i++)
        {
            if (knotObjects[i] != null)
            {
                knotObjects[i].transform.position = knotPositions[i];
            }
        }

        for (int i = 0; i < tangentInObjects.Count && i < tangentInPositions.Count; i++)
        {
            if (tangentInObjects[i] != null)
            {
                tangentInObjects[i].transform.position = tangentInPositions[i];
            }
        }

        for (int i = 0; i < tangentOutObjects.Count && i < tangentOutPositions.Count; i++)
        {
            if (tangentOutObjects[i] != null)
            {
                tangentOutObjects[i].transform.position = tangentOutPositions[i];
            }
        }
    }
}
