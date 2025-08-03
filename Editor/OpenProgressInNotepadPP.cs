using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class OpenProgressInNotepadPP {
        [MenuItem("Game/Open Save JSON in Notepad++")]
        public static void OpenProgressFile() {
            // Path to saved file
            string jsonPath = Path.Combine(Application.persistentDataPath, "progress.json");

            // Path to Notepad++ (update if needed)
            string notepadPPPath = @"C:\Program Files\Notepad++\notepad++.exe";

            if (!File.Exists(jsonPath)) {
                UnityEngine.Debug.LogWarning($"JSON file not found: {jsonPath}");
                return;
            }

            if (!File.Exists(notepadPPPath)) {
                UnityEngine.Debug.LogError($"Notepad++ not found at: {notepadPPPath}");
                return;
            }

            // Launch Notepad++ with the JSON file
            Process.Start(notepadPPPath, $"\"{jsonPath}\"");
        }

        [MenuItem("Game/Danger/Delete progress")]
        public static void DeleteProgress() {
            string path = Path.Combine(Application.persistentDataPath, "progress.json");

            if (File.Exists(path)) {
                File.Delete(path);
                UnityEngine.Debug.Log("Save file deleted.");
            }
            else {
                UnityEngine.Debug.Log("No save file found.");
            }
        }
    }
}
