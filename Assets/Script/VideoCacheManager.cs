using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class VideoCacheManager : MonoBehaviour
{
    [Header("Default Cache")]
    [SerializeField] private string cachedFileName = "antarctica.mp4";

    public bool IsDownloading { get; private set; }
    public bool IsReady { get; private set; }

    public string LocalVideoPath
    {
        get
        {
            return GetLocalVideoPath(cachedFileName);
        }
    }

    public event Action<string> VideoReady;
    public event Action<float> DownloadProgressChanged;
    public event Action<string> DownloadFailed;

    public string GetLocalVideoPath(string fileName)
    {
        return Path.Combine(
            Application.persistentDataPath,
            fileName);
    }

    public bool HasCachedVideo()
    {
        return HasCachedVideo(cachedFileName);
    }

    public bool HasCachedVideo(string fileName)
    {
        return File.Exists(
            GetLocalVideoPath(fileName));
    }


    public void PrepareVideo(
        string fileName,
        string remoteUrl)
    {
        string localPath =
            GetLocalVideoPath(fileName);

        if (File.Exists(localPath))
        {
            IsReady = true;

            Debug.Log(
                $"VideoCacheManager: Cached video found at {localPath}");

            VideoReady?.Invoke(localPath);

            return;
        }

        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            string message =
                $"VideoCacheManager: {fileName} is not cached and no remote URL was supplied.";

            Debug.LogError(message);

            DownloadFailed?.Invoke(message);

            return;
        }

        StartCoroutine(
            DownloadVideo(
                fileName,
                remoteUrl));
    }

    private IEnumerator DownloadVideo(
        string fileName,
        string remoteUrl)
    {
        IsDownloading = true;
        IsReady = false;

        string localPath =
            GetLocalVideoPath(fileName);

        string temporaryPath =
            localPath + ".download";

        Debug.Log(
            $"VideoCacheManager: Downloading {fileName} from {remoteUrl}");

        using (UnityWebRequest request =
               UnityWebRequest.Get(remoteUrl))
        {
            request.downloadHandler =
                new DownloadHandlerFile(
                    temporaryPath);

            UnityWebRequestAsyncOperation operation =
                request.SendWebRequest();

            while (!operation.isDone)
            {
                DownloadProgressChanged?.Invoke(
                    request.downloadProgress);

                yield return null;
            }

            if (request.result !=
                UnityWebRequest.Result.Success)
            {
                string message =
                    $"VideoCacheManager download failed: {request.error}";

                Debug.LogError(message);

                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);

                IsDownloading = false;

                DownloadFailed?.Invoke(message);

                yield break;
            }
        }

        if (File.Exists(localPath))
            File.Delete(localPath);

        File.Move(
            temporaryPath,
            localPath);

        IsDownloading = false;
        IsReady = true;

        Debug.Log(
            $"VideoCacheManager: Video cached at {localPath}");

        VideoReady?.Invoke(localPath);
    }
}