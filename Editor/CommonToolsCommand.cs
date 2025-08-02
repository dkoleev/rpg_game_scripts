using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor {
    public class CommonToolsCommand {
        [MenuItem("Tools/Load and Play Boot Scene")]
        public static void LoadAndPlayBootScene() {
            // Path to your Boot scene
            string scenePath = "Assets/Scenes/Boot.unity";

            // Save current modified scenes (optional prompt)
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                // Open the Boot scene
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                // Start Play Mode
                EditorApplication.isPlaying = true;
            }
        }
    }
}
