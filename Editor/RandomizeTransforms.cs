using UnityEngine;
using UnityEditor;

public class RandomizeTransforms : EditorWindow
{
    Vector3 positionAmount = Vector3.zero;
    Vector3 rotationAmount = Vector3.zero;
    Vector3 scaleAmount = Vector3.zero;
    int randomSeed = 0;

    [MenuItem("CP_Tools/Randomize Transforms")]
    static void Init()
    {
        RandomizeTransforms window = (RandomizeTransforms)EditorWindow.GetWindow(typeof(RandomizeTransforms));
        window.titleContent = new GUIContent("Randomize Transforms");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Randomization Amounts", EditorStyles.boldLabel);

        positionAmount = EditorGUILayout.Vector3Field("Position Amount", positionAmount);
        rotationAmount = EditorGUILayout.Vector3Field("Rotation Amount", rotationAmount);
        scaleAmount = EditorGUILayout.Vector3Field("Scale Amount", scaleAmount);

        GUILayout.Space(10);

        randomSeed = EditorGUILayout.IntField("Random Seed", randomSeed);

        if (GUILayout.Button("Randomize Selected"))
        {
            RandomizeSelected();
        }
    }

    void RandomizeSelected()
    {
        Random.InitState(randomSeed);

        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Randomize Transform");

            // Randomize Position
            Vector3 pos = obj.transform.localPosition;
            pos.x += Random.Range(-positionAmount.x, positionAmount.x);
            pos.y += Random.Range(-positionAmount.y, positionAmount.y);
            pos.z += Random.Range(-positionAmount.z, positionAmount.z);
            obj.transform.localPosition = pos;

            // Randomize Rotation
            Vector3 rot = obj.transform.localEulerAngles;
            rot.x += Random.Range(-rotationAmount.x, rotationAmount.x);
            rot.y += Random.Range(-rotationAmount.y, rotationAmount.y);
            rot.z += Random.Range(-rotationAmount.z, rotationAmount.z);
            obj.transform.localEulerAngles = rot;

            // Randomize Scale
            Vector3 scale = obj.transform.localScale;
            scale.x += Random.Range(-scaleAmount.x, scaleAmount.x);
            scale.y += Random.Range(-scaleAmount.y, scaleAmount.y);
            scale.z += Random.Range(-scaleAmount.z, scaleAmount.z);
            obj.transform.localScale = scale;

            EditorUtility.SetDirty(obj);
        }
    }
}
