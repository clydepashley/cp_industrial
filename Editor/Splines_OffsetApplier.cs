using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;
using System.Collections.Generic;

public class Splines_OffsetApplier : EditorWindow
{
    private GameObject rootObject;
    private float offsetValue = 0.1f;

    [MenuItem("CP_Tools/Spline Offset Applier")]
    public static void ShowWindow()
    {
        GetWindow<Splines_OffsetApplier>("Spline Offset Applier");
    }

    private void OnGUI()
    {
        GUILayout.Label("Apply staggered spline Start Offsets (reversed)", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true);
        offsetValue = EditorGUILayout.FloatField("Offset Value", offsetValue);

        if (GUILayout.Button("Apply Offsets"))
        {
            if (rootObject == null)
            {
                Debug.LogWarning("Please assign a root object.");
                return;
            }

            var chain = new List<Transform>();
            CollectSplineAnimateChain(rootObject.transform, chain);

            if (chain.Count == 0)
            {
                Debug.LogWarning("No SplineAnimate components found in the chain.");
                return;
            }

            // Apply offsets in reverse order
            int count = chain.Count;
            for (int i = 0; i < count; i++)
            {
                var splineAnimate = chain[i].GetComponent<SplineAnimate>();
                if (splineAnimate != null)
                {
                    float startOffset = offsetValue * (count - 1 - i);
                    splineAnimate.StartOffset = startOffset;
                    EditorUtility.SetDirty(splineAnimate);
                }
            }

            Debug.Log($"Applied StartOffsets to {count} bones in reversed order.");
        }
    }

    // Collects the chain of Transforms with SplineAnimate components by following first child with SplineAnimate
    private void CollectSplineAnimateChain(Transform current, List<Transform> chain)
    {
        var splineAnimate = current.GetComponent<SplineAnimate>();
        if (splineAnimate != null)
        {
            chain.Add(current);
        }
        else
        {
            // If current doesn't have SplineAnimate, do not add or continue
            return;
        }

        // Find first child with SplineAnimate
        Transform next = null;
        foreach (Transform child in current)
        {
            if (child.GetComponent<SplineAnimate>() != null)
            {
                next = child;
                break;
            }
        }

        if (next != null)
        {
            CollectSplineAnimateChain(next, chain);
        }
    }
}