using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetBackgroundColorForAllScenes
{
    [MenuItem("GraceTools/Set Pink Background for All Scenes")]
    public static void ApplyPinkBackground()
    {
        // Get all scene paths in the project
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            // Skip the scene named "End"
            if (scenePath.Contains("End")) continue;

            if (!scenePath.Contains("Scenes")) continue;

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.backgroundColor = new Color(215, 123, 186);
                EditorUtility.SetDirty(mainCam);
                Debug.Log($"Set pink background in {scene.name}");

                // Force scene view repaint
                SceneView.RepaintAll();
            }

            // Save the scene
            EditorSceneManager.SaveScene(scene);
        }

        Debug.Log("Done applying pink backgrounds to all scenes (except 'End').");
    }
}
