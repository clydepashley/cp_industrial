using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;
using System.Collections.Generic;

public class Splines_OffsetApplier : EditorWindow
{
    private GameObject rootObject;
    private float offsetValue = 0.1f;

    // Animation curve creation fields
    private AnimationClip animationClip;
    private float timeOffset = 0.1f;

    // New fields for base clip creation
    private GameObject animatorRootObject;
    private GameObject splineAnimateObject;
    private string newClipPath = "Assets/";

    [MenuItem("CP_Tools/Spline Offset Applier")]
    public static void ShowWindow()
    {
        GetWindow<Splines_OffsetApplier>("Spline Offset Applier");
    }

    private void OnGUI()
    {
        GUILayout.Label("Apply staggered spline Start Offsets (reversed)", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true);
        offsetValue = EditorGUILayout.FloatField("Offset Value (StartOffset)", offsetValue);

        if (GUILayout.Button("Apply StartOffsets"))
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

            Debug.Log($"Applied StartOffsets to {chain.Count} bones in reversed order.");
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Create Animation Curves with Time Offset", EditorStyles.boldLabel);

        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false);
        timeOffset = EditorGUILayout.FloatField("Time Offset (seconds)", timeOffset);

        if (GUILayout.Button("Create Offset Animation Curves"))
        {
            if (rootObject == null)
            {
                Debug.LogWarning("Please assign a root object.");
                return;
            }
            if (animationClip == null)
            {
                Debug.LogWarning("Please assign an animation clip.");
                return;
            }

            CreateOffsetAnimationCurves(rootObject.transform, animationClip, timeOffset);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Create Base Animation Clip", EditorStyles.boldLabel);

        animatorRootObject = (GameObject)EditorGUILayout.ObjectField("Animator Root Object", animatorRootObject, typeof(GameObject), true);
        splineAnimateObject = (GameObject)EditorGUILayout.ObjectField("SplineAnimate Object", splineAnimateObject, typeof(GameObject), true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Path", GUILayout.MaxWidth(70));
        EditorGUILayout.SelectableLabel(newClipPath, GUILayout.Height(16));
        if (GUILayout.Button("Browse", GUILayout.MaxWidth(80)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Animation Clip",
                "NewSplineAnimateClip.anim",
                "anim",
                "Choose location to save the new animation clip",
                newClipPath);

            if (!string.IsNullOrEmpty(path))
            {
                newClipPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create Base Animation Clip"))
        {
            if (animatorRootObject == null)
            {
                Debug.LogWarning("Please assign the Animator root object.");
                return;
            }
            if (splineAnimateObject == null)
            {
                Debug.LogWarning("Please assign the GameObject with the SplineAnimate component.");
                return;
            }
            if (string.IsNullOrEmpty(newClipPath))
            {
                Debug.LogWarning("Please specify a valid save path for the animation clip.");
                return;
            }

            CreateBaseAnimationClip(animatorRootObject.transform, splineAnimateObject.transform, newClipPath);
        }
    }

    private void CollectSplineAnimateChain(Transform current, List<Transform> chain)
    {
        var splineAnimate = current.GetComponent<SplineAnimate>();
        if (splineAnimate != null)
        {
            chain.Add(current);
        }
        else
        {
            return;
        }

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

    private void CreateOffsetAnimationCurves(Transform root, AnimationClip clip, float timeOffset)
    {
        var chain = new List<Transform>();
        CollectSplineAnimateChain(root, chain);

        if (chain.Count == 0)
        {
            Debug.LogWarning("No SplineAnimate components found in the chain.");
            return;
        }

        Transform clipRoot = root.root;
        string rootPath = AnimationUtility.CalculateTransformPath(root, clipRoot);

        // Change here: use wrapper component type and property name without 'm_'
        const string propertyName = "NormalizedTime";

        EditorCurveBinding rootBinding = new EditorCurveBinding
        {
            path = rootPath,
            type = typeof(Splines_AnimateNormalizedTimeWrapper),  // <-- wrapper type here
            propertyName = propertyName
        };

        AnimationCurve parentCurve = AnimationUtility.GetEditorCurve(clip, rootBinding);

        if (parentCurve == null)
        {
            Debug.LogWarning($"No animation curve found for '{root.name}' NormalizedTime in the clip.");
            return;
        }

        Undo.RecordObject(clip, "Create Offset Animation Curves");

        int count = chain.Count;
        for (int i = 0; i < count; i++)
        {
            Transform bone = chain[i];
            string bonePath = AnimationUtility.CalculateTransformPath(bone, clipRoot);

            EditorCurveBinding binding = new EditorCurveBinding
            {
                path = bonePath,
                type = typeof(Splines_AnimateNormalizedTimeWrapper),  // <-- wrapper type here
                propertyName = propertyName
            };

            AnimationCurve offsetCurve = new AnimationCurve();

            foreach (var key in parentCurve.keys)
            {
                float newTime = key.time - i * timeOffset;
                if (newTime < 0f) newTime = 0f;

                Keyframe newKey = new Keyframe(newTime, key.value, key.inTangent, key.outTangent)
                {
                    tangentMode = key.tangentMode
                };
                offsetCurve.AddKey(newKey);
            }

            AnimationUtility.SetEditorCurve(clip, binding, offsetCurve);
        }

        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();

        Debug.Log($"Created offset animation curves for {count} bones in clip '{clip.name}'.");
    }

    private void CreateBaseAnimationClip(Transform animatorRoot, Transform splineAnimateTransform, string savePath)
    {
        var splineAnimate = splineAnimateTransform.GetComponent<SplineAnimate>();
        if (splineAnimate == null)
        {
            Debug.LogWarning("The specified GameObject does not have a SplineAnimate component.");
            return;
        }

        // Calculate relative path from animator root to the wrapper component GameObject
        // Assuming the wrapper is on the same GameObject as splineAnimate
        string relativePath = AnimationUtility.CalculateTransformPath(splineAnimateTransform, animatorRoot);

        AnimationClip clip = new AnimationClip();
        clip.name = System.IO.Path.GetFileNameWithoutExtension(savePath);

        EditorCurveBinding binding = new EditorCurveBinding
        {
            path = relativePath,
            type = typeof(Splines_AnimateNormalizedTimeWrapper),  // <-- wrapper type here
            propertyName = "NormalizedTime"                     // <-- wrapper property name
        };

        // Create a linear curve from 0 to 1 over 2 seconds (default duration)
        AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 2f, 1f);

        AnimationUtility.SetEditorCurve(clip, binding, curve);

        // Save the clip asset
        AssetDatabase.CreateAsset(clip, savePath);
        AssetDatabase.SaveAssets();

        Debug.Log($"Created base animation clip '{clip.name}' at '{savePath}'.");
    }
}