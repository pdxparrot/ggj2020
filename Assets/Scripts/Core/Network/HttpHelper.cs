using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace pdxpartyparrot.Core.Network
{
    // TODO: this is totally untested
    public static class HttpHelper
    {
#region GET
        public static IEnumerator<float> GetText(string url, Action<string> success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke(www.downloadHandler.text);
            }
        }

        public static IEnumerator<float> GetBytes(string url, Action<byte[]> success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke(www.downloadHandler.data);
            }
        }

        public static IEnumerator<float> GetAssetBundle(string url, Action<AssetBundle> success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke(((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle);
            }
        }

        public static IEnumerator<float> GetTexture(string url, Action<Texture> success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke(((DownloadHandlerTexture)www.downloadHandler).texture);
            }
        }

        public static IEnumerator<float> GetAudioClip(string url, Action<AudioClip> success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke(((DownloadHandlerAudioClip)www.downloadHandler).audioClip);
            }
        }
#endregion

#region POST
        public static IEnumerator<float> Post(string url, List<IMultipartFormSection> formData, Action success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Post(url, formData);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke();
            }
        }
#endregion

#region PUT
        public static IEnumerator<float> Put(string url, byte[] data, Action success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Put(url, data);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke();
            }
        }
#endregion

#region DELETE
        public static IEnumerator<float> Delete(string url, Action success, Action<string> failure)
        {
            UnityWebRequest www = UnityWebRequest.Delete(url);
            AsyncOperation asyncOp =  www.SendWebRequest();
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }
     
            if(www.isNetworkError || www.isHttpError) {
                failure?.Invoke(www.error);
            } else {
                success?.Invoke();
            }
        }
#endregion
    }
}
