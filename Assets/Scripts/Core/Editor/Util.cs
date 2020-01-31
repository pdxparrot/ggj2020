using System.IO;
using System.Threading;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace pdxpartyparrot.Core.Editor
{
    // TODO: some of this should move to Core.Util.EditorUtils so game code can use it in editor mode
    public static class Util
    {
        public const string ProjectSettingsAssetPath = "ProjectSettings/ProjectSettings.asset";

#region Assets
        // creates the asset folder only if it doesn't already exist
        public static void CreateAssetFolder(string parentFolder, string newFolderName)
        {
            if(!Directory.Exists($"{parentFolder}/{newFolderName}")) {
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }

        // creates the given asset
        // removes the asset first if it already exists
        public static void CreateAsset(UnityEngine.Object obj, string path)
        {
            if(File.Exists(path)) {
                File.Delete(path);
            }

            AssetDatabase.CreateAsset(obj, path);
        }

        public static void DownloadAssetToFile(string url, string path)
        {
            Debug.Log($"Downloading asset from {url} to {path}...");

            UnityWebRequest www = UnityWebRequest.Get(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                Thread.Sleep(500);
            }
     
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log($"Failed to download asset from {url}: {www.error}");
                return;
            }

            FileInfo fileInfo = new FileInfo(path);
            if(fileInfo.Exists) {
                fileInfo.Delete();
            }

            DirectoryInfo dirInfo = fileInfo.Directory;
            if(null != dirInfo && !dirInfo.Exists) {
                dirInfo.Create();
            }

            File.WriteAllBytes(path, www.downloadHandler.data);

            AssetDatabase.ImportAsset(path);
        }

        public static void DownloadTextureToFile(string url, string path, TextureImporterType type)
        {
            DownloadAssetToFile(url, path);

            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
            textureImporter.textureType = type;
            textureImporter.SaveAndReimport();
        }
#endregion

#region Packages
        public static void AddPackage(string identifier)
        {
            Debug.Log($"Adding package {identifier}...");

            AddRequest request = Client.Add(identifier);
            while(!request.IsCompleted) {
                Thread.Sleep(500);
            }

            if(StatusCode.Failure == request.Status) {
                Debug.Log($"Failed to add package {identifier}: {request.Error.message}");
                return;
            }
        }

        public static void RemovePackage(string packageName)
        {
            Debug.Log($"Removing package {packageName}...");

            RemoveRequest request = Client.Remove(packageName);
            while(!request.IsCompleted) {
                Thread.Sleep(500);
            }

            if(StatusCode.Failure == request.Status) {
                Debug.Log($"Failed to add package {packageName}: {request.Error.message}");
                return;
            }
        }
#endregion

        // TODO: this should move to the core filesystem utils
        public static void CreateEmptyFile(string path)
        {
            File.WriteAllText(path, "");
        }
    }
}
