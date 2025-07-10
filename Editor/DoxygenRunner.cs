using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class DoxygenRunner
    {
        [MenuItem("Tools/Run Doxygen")]
        public static void RunDoxygen()
        {
            string projectPath = Application.dataPath.Replace("/Assets", "");
            string doxyfilePath = Path.Combine(projectPath, "Doxyfile");

            if (!File.Exists(doxyfilePath))
            {
                UnityEngine.Debug.LogError("Doxyfile not found at project root. Generate one with 'doxygen -g' first.");
                return;
            }

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "doxygen",
                Arguments = $"\"{doxyfilePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process process = new Process { StartInfo = processInfo };

            try
            {
                process.OutputDataReceived += (sender, e) => UnityEngine.Debug.Log(e.Data);
                process.ErrorDataReceived += (sender, e) => UnityEngine.Debug.LogError(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                UnityEngine.Debug.Log("Doxygen finished.");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Failed to run Doxygen: " + e.Message);
            }
        }
    }
}