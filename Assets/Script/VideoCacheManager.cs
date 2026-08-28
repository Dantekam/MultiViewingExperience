using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class VideoCacheManager : MonoBehaviour
{
    [Header("Cache")]
    [SerializeField] private string cachedFileName = "antarctica.mp4";

    public bool IsDownloading { get; private set; }
    public bool IsReady { get; private set; }

    public string LocalVideoPath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                cachedFileName);
        }
    }

    public event Action<string> VideoReady;
    public event Action<float> DownloadProgressChanged;
    public event Action<string> DownloadFailed;

    public bool HasCachedVideo()
    {
        return File.Exists(LocalVideoPath);
    }

    public void PrepareVideo(string remoteUrl)
    {
        if (HasCachedVideo())
        {
            IsReady = true;

            Debug.Log(
                $"VideoCacheManager: Cached video found at {LocalVideoPath}");

            VideoReady?.Invoke(LocalVideoPath);

            return;
        }

        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            string message =
                "VideoCacheManager: Remote video URL is missing.";

            Debug.LogError(message);

            DownloadFailed?.Invoke(message);

            return;
        }

        StartCoroutine(
            DownloadVideo(remoteUrl));
    }

    private IEnumerator DownloadVideo(string remoteUrl)
    {
        IsDownloading = true;
        IsReady = false;

        Debug.Log(
            $"VideoCacheManager: Downloading video from {remoteUrl}");

        string temporaryPath =
            LocalVideoPath + ".download";

        using (UnityWebRequest request =
               UnityWebRequest.Get(remoteUrl))
        {
            request.downloadHandler =
                new DownloadHandlerFile(temporaryPath);

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

        if (File.Exists(LocalVideoPath))
            File.Delete(LocalVideoPath);

        File.Move(
            temporaryPath,
            LocalVideoPath);

        IsDownloading = false;
        IsReady = true;

        Debug.Log(
            $"VideoCacheManager: Video cached at {LocalVideoPath}");

        VideoReady?.Invoke(LocalVideoPath);
    }
}