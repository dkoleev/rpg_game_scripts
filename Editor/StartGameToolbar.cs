#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine.SceneManagement;

namespace Darkness.Editor {
    [Overlay(typeof(SceneView), "Boot Controls")]
    public class BootToolbarOverlay : ToolbarOverlay
    {
        public BootToolbarOverlay() : base(BootPlayButton.ID) { }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    class BootPlayButton : EditorToolbarButton {
        public const string ID = "BootToolbar/PlayBoot";

        private const string BootScenePath = "Assets/Scenes/Boot.unity";
        private const string PreviousSceneKey = "PreviousScenePath";

        public BootPlayButton() {
            text = "▶ Game";
            tooltip = "Load and Play Boot Scene";

            clicked += () => {
                string currentScene = SceneManager.GetActiveScene().path;
                EditorPrefs.SetString(PreviousSceneKey, currentScene);

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    EditorSceneManager.OpenScene(BootScenePath);
                    EditorApplication.isPlaying = true;
                }
            };

            // Restore scene after play mode ends
            EditorApplication.playModeStateChanged += (PlayModeStateChange state) => {
                if (state == PlayModeStateChange.EnteredEditMode) {
                    if (EditorPrefs.HasKey(PreviousSceneKey)) {
                        string prevScene = EditorPrefs.GetString(PreviousSceneKey);
                        if (!string.IsNullOrEmpty(prevScene))
                            EditorSceneManager.OpenScene(prevScene);
                        EditorPrefs.DeleteKey(PreviousSceneKey);
                    }
                }
            };
        }
    }
#endif
}