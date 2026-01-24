using UnityEngine;
using UnityEditor;
using System.IO;

public class CombineTexturesEditor : EditorWindow
{
    Texture2D rgbTexture;
    Texture2D alphaTexture;

    [MenuItem("CP_Tools/Combine RGB and Alpha Textures")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CombineTexturesEditor), false, "Combine Textures");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Textures", EditorStyles.boldLabel);

        rgbTexture = (Texture2D)EditorGUILayout.ObjectField("RGB Texture", rgbTexture, typeof(Texture2D), false);
        alphaTexture = (Texture2D)EditorGUILayout.ObjectField("Alpha Texture", alphaTexture, typeof(Texture2D), false);

        if (GUILayout.Button("Combine and Save"))
        {
            if (rgbTexture == null || alphaTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both textures.", "OK");
                return;
            }

            CombineAndSaveTextures(rgbTexture, alphaTexture);
        }
    }

    void CombineAndSaveTextures(Texture2D rgbTex, Texture2D alphaTex)
    {
        int width = rgbTex.width;
        int height = rgbTex.height;

        if (alphaTex.width != width || alphaTex.height != height)
        {
            EditorUtility.DisplayDialog("Error", "Textures must be the same size.", "OK");
            return;
        }

        Texture2D combined = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] rgbPixels = rgbTex.GetPixels();
        Color[] alphaPixels = alphaTex.GetPixels();

        for (int i = 0; i < rgbPixels.Length; i++)
        {
            Color rgb = rgbPixels[i];
            float alpha = alphaPixels[i].grayscale; // use grayscale value from alpha texture
            combined.SetPixel(i % width, i / width, new Color(rgb.r, rgb.g, rgb.b, alpha));
        }

        combined.Apply();

        string path = EditorUtility.SaveFilePanel("Save Combined Texture", "Assets", "CombinedTexture", "png");

        if (!string.IsNullOrEmpty(path))
        {
            byte[] pngData = combined.EncodeToPNG();
            File.WriteAllBytes(path, pngData);
            AssetDatabase.Refresh();
            Debug.Log("Texture saved to: " + path);
        }
    }
}