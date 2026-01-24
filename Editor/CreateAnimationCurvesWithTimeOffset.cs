using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateAnimationCurvesWithTimeOffset : EditorWindow
{
    GameObject animatorRoot;
    GameObject firstSplineAnimator;
    AnimationClip animationClip;
    float timeOffset = 0.1f;

    [MenuItem("CP_Tools/Create Animation Curves With Time Offset")]
    static void Init()
    {
        CreateAnimationCurvesWithTimeOffset window = (CreateAnimationCurvesWithTimeOffset)GetWindow(typeof(CreateAnimationCurvesWithTimeOffset));
        window.titleContent = new GUIContent("Create Curves With Offset");
        window.Show();
    }

    void OnGUI()
    {
        GUIStyle wrapStyle = new GUIStyle(EditorStyles.label);
        wrapStyle.wordWrap = true;
        wrapStyle.richText = true;
        GUILayout.Label("<color=yellow>Deprecated:</color> merged with Splines_OffsetApplier, will delete when tested", wrapStyle);
        GUILayout.Label("Parameters", EditorStyles.boldLabel);

        animatorRoot = (GameObject)EditorGUILayout.ObjectField("Animator Root", animatorRoot, typeof(GameObject), true);
        firstSplineAnimator = (GameObject)EditorGUILayout.ObjectField("First Spline Animator", firstSplineAnimator, typeof(GameObject), true);
        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false);
        timeOffset = EditorGUILayout.FloatField("Time Offset (seconds)", timeOffset);

        if (GUILayout.Button("Create Animation Curves With Offset"))
        {
            if (animatorRoot == null || firstSplineAnimator == null || animationClip == null)
            {
                Debug.LogError("Please assign all parameters.");
                return;
            }

            CreateCurvesWithOffset();
        }
    }

    void CreateCurvesWithOffset()
    {
        var firstWrapper = firstSplineAnimator.GetComponent<Splines_AnimateNormalizedTimeWrapper>();
        if (firstWrapper == null)
        {
            Debug.LogError("FirstSplineAnimator does not have Splines_AnimateNormalizedTimeWrapper component.");
            return;
        }

        // Find the curve binding for _normalizedTime on the template clip for the firstSplineAnimator
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animationClip);
        EditorCurveBinding targetBinding = default;
        bool found = false;

        foreach (var binding in bindings)
        {
            if (binding.propertyName.Contains("_normalizedTime"))
            {
                targetBinding = binding;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogError("Could not find _normalizedTime animation curve in the animation clip.");
            return;
        }

        AnimationCurve originalCurve = AnimationUtility.GetEditorCurve(animationClip, targetBinding);
        if (originalCurve == null)
        {
            Debug.LogError("Failed to get animation curve for _normalizedTime.");
            return;
        }

        // Find all children of animatorRoot that have Splines_AnimateNormalizedTimeWrapper component
        List<GameObject> splineObjects = new List<GameObject>();
        GetChildrenWithComponent(animatorRoot.transform, splineObjects);

        // Ensure firstSplineAnimator is first in the list
        if (!splineObjects.Contains(firstSplineAnimator))
            splineObjects.Insert(0, firstSplineAnimator);
        else
        {
            // Move firstSplineAnimator to front
            splineObjects.Remove(firstSplineAnimator);
            splineObjects.Insert(0, firstSplineAnimator);
        }

        Undo.RecordObject(animationClip, "Modify Animation Clip with Time Offset Curves");

        // Remove existing curves for _normalizedTime on all splineObjects to avoid duplicates
        foreach (var go in splineObjects)
        {
            string path = AnimationUtility.CalculateTransformPath(go.transform, animatorRoot.transform);
            EditorCurveBinding bindingToRemove = new EditorCurveBinding
            {
                path = path,
                propertyName = targetBinding.propertyName,
                type = targetBinding.type
            };
            AnimationUtility.SetEditorCurve(animationClip, bindingToRemove, null);
        }

        // Add shifted curves for each child
        for (int i = 0; i < splineObjects.Count; i++)
        {
            GameObject go = splineObjects[i];
            var wrapper = go.GetComponent<Splines_AnimateNormalizedTimeWrapper>();
            if (wrapper == null) continue;

            AnimationCurve shiftedCurve = new AnimationCurve();

            foreach (var key in originalCurve.keys)
            {
                Keyframe shiftedKey = new Keyframe(key.time + timeOffset * i, key.value, key.inTangent, key.outTangent);
                shiftedCurve.AddKey(shiftedKey);
            }

            string relativePath = AnimationUtility.CalculateTransformPath(go.transform, animatorRoot.transform);

            EditorCurveBinding newBinding = new EditorCurveBinding
            {
                path = relativePath,
                propertyName = targetBinding.propertyName,
                type = targetBinding.type
            };

            AnimationUtility.SetEditorCurve(animationClip, newBinding, shiftedCurve);
        }

        EditorUtility.SetDirty(animationClip);
        AssetDatabase.SaveAssets();

        Debug.Log("Modified animation clip with time offset curves for children.");
    }

    void GetChildrenWithComponent(Transform parent, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<Splines_AnimateNormalizedTimeWrapper>() != null)
            {
                list.Add(child.gameObject);
            }
            GetChildrenWithComponent(child, list);
        }
    }
}