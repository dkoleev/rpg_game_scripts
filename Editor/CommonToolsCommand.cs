using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor {
    public class CommonToolsCommand {
        [MenuItem("Game/Load and Play Boot Scene &r")]
        public static void LoadAndPlayBootScene() {
            // Path to your Boot scene
            string scenePath = "Assets/Scenes/Boot.unity";
            OpenScene(scenePath, true);
        }
        
        [MenuItem("Game/Open Camera Scene")]
        public static void OpenCameraScene() {
            OpenScene("Assets/Scenes/Camera&Events&Lighting.unity", false);
        }
        
        [MenuItem("Game/Open Boot Scene")]
        public static void OpenBootScene() {
            OpenScene("Assets/Scenes/Boot.unity", false);
        }

        private static void OpenScene(string scenePath, bool startPlayMode) {
            // Save current modified scenes (optional prompt)
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                // Open the Boot scene
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                // Start Play Mode
                EditorApplication.isPlaying = startPlayMode;
            }
            
        }
    }
}
