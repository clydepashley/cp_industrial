// Save as: Assets/Editor/MaterialVariantTool.cs
// Usage: Window -> Material Variant Tool
using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialVariantTool : EditorWindow
{
    Material masterMaterial;
    Material variantMaterial;
    string newVariantPath = "Assets/NewMaterialVariant.mat";
    bool createNewVariant = false;

    [MenuItem("CP_Tools/Material Variant Tool")]
    public static void ShowWindow()
    {
        GetWindow<MaterialVariantTool>("Material Variant Tool");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Convert / Create Material Variant", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        masterMaterial = (Material)EditorGUILayout.ObjectField("Master Material", masterMaterial, typeof(Material), false);
        variantMaterial = (Material)EditorGUILayout.ObjectField("Variant Material (optional)", variantMaterial, typeof(Material), false);

        createNewVariant = EditorGUILayout.ToggleLeft("Create new variant asset (if Variant Material is empty)", createNewVariant);

        using (new EditorGUI.DisabledScope(!createNewVariant))
        {
            EditorGUILayout.BeginHorizontal();
            newVariantPath = EditorGUILayout.TextField(newVariantPath);
            if (GUILayout.Button("Choose...", GUILayout.Width(80)))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save new material variant", Path.GetFileName(newVariantPath), "mat", "Choose location to save variant");
                if (!string.IsNullOrEmpty(path))
                    newVariantPath = path;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(masterMaterial == null || (!createNewVariant && variantMaterial == null && variantMaterial == null));
        if (GUILayout.Button(createNewVariant ? "Create & Parent Variant" : "Convert Selected to Variant", GUILayout.Height(40)))
        {
            if (masterMaterial == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Master Material.", "OK");
            }
            else
            {
                if (createNewVariant)
                    CreateAndParentVariant(masterMaterial, newVariantPath);
                else
                    ConvertToVariant(masterMaterial, variantMaterial);
            }
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Notes:\n- Material variants are editor-only (hierarchy flattened at build/runtime).\n- This sets the asset's parent via material.parent = master.\n- Requires Unity Editor that supports Material Variants (Unity 2022+ recommended).", MessageType.Info);
    }

    static void CreateAndParentVariant(Material master, string assetPath)
    {
        if (master == null || string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Master material or asset path is invalid.");
            return;
        }

        // If file exists, confirm overwrite
        if (File.Exists(assetPath))
        {
            if (!EditorUtility.DisplayDialog("Overwrite?", $"A file already exists at {assetPath}. Overwrite?", "Yes", "No"))
                return;
        }

        // Create a new material that uses the master's shader and properties as a starting point
        Material variant = new Material(master);

        // Important: create the asset first
        AssetDatabase.CreateAsset(variant, assetPath);
        AssetDatabase.ImportAsset(assetPath); // ensure it's imported as an asset

        // Set parent (editor-only API)
        variant = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (variant == null)
        {
            Debug.LogError("Failed to create/load the new variant asset at: " + assetPath);
            return;
        }

        Undo.RegisterCompleteObjectUndo(variant, "Create Material Variant");
        variant.parent = master; // make it a variant of master
        EditorUtility.SetDirty(variant);
        AssetDatabase.SaveAssets();

        Selection.activeObject = variant;
        Debug.Log($"Created material variant '{variant.name}' at '{assetPath}' with parent '{master.name}'.");
    }

    static void ConvertToVariant(Material master, Material variant)
    {
        if (master == null || variant == null)
        {
            Debug.LogError("Master and Variant must both be assigned (or use Create).");
            return;
        }

        // Prevent circular parenting
        if (master == variant)
        {
            EditorUtility.DisplayDialog("Invalid", "Master and Variant are the same material.", "OK");
            return;
        }

        // Register Undo for the variant asset
        Undo.RecordObject(variant, "Convert to Material Variant");
        variant.parent = master;
        EditorUtility.SetDirty(variant);
        AssetDatabase.SaveAssets();

        Debug.Log($"Converted material '{variant.name}' to be a variant of '{master.name}'.");
    }
}
