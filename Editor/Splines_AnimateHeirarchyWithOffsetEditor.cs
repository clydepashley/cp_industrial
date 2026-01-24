using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Splines_AnimateHeirarchyWithOffset))]
public class Splines_AnimateHeirarchyWithOffsetEditor : Editor
{
    SerializedProperty splineAnimatorsProp;
    SerializedProperty timeOffsetProp;
    SerializedProperty normalizedTimeProp;

    private void OnEnable()
    {
        splineAnimatorsProp = serializedObject.FindProperty("splineAnimators");
        timeOffsetProp = serializedObject.FindProperty("timeOffset");
        normalizedTimeProp = serializedObject.FindProperty("normalizedTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw splineAnimators and timeOffset normally
        EditorGUILayout.PropertyField(splineAnimatorsProp, true);
        EditorGUILayout.PropertyField(timeOffsetProp);

        // Calculate dynamic max for normalizedTime slider
        int count = splineAnimatorsProp.arraySize - 1;
        float timeOffset = timeOffsetProp.floatValue;
        float maxNormalizedTime = 1f + (count * timeOffset);

        // Clamp normalizedTime to valid range
        normalizedTimeProp.floatValue = Mathf.Clamp(normalizedTimeProp.floatValue, 0f, maxNormalizedTime);

        // Draw slider with dynamic range
        normalizedTimeProp.floatValue = EditorGUILayout.Slider("Normalized Time", normalizedTimeProp.floatValue, 0f, maxNormalizedTime);

        serializedObject.ApplyModifiedProperties();
    }
}