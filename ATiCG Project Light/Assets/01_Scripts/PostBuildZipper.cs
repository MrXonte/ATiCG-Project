#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.IO.Compression;
using UnityEditor;

public class PostBuildZipper : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.WebGL)
        {
            string buildPath = report.summary.outputPath;
            string directory = Path.GetDirectoryName(buildPath);

            DeleteExistingZips(directory);

            string zipPath = GetZipPath(buildPath);
            ZipFile.CreateFromDirectory(buildPath, zipPath);
            Debug.Log("Build zipped to: " + zipPath);
        }
    }

    private void DeleteExistingZips(string directory)
    {
        var zipFiles = Directory.GetFiles(directory, "*.zip");
        foreach (var file in zipFiles)
        {
            try
            {
                File.Delete(file);
                Debug.Log("Deleted old zip file: " + file);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error deleting file: " + file + ". Exception: " + ex.Message);
            }
        }
    }

    private string GetZipPath(string buildPath)
    {
        string directory = Path.GetDirectoryName(buildPath);
        string zipName = System.DateTime.Now.ToString("yyyy_MM_dd_HHmm") + ".zip";
        return Path.Combine(directory, zipName);
    }
}
#endif
