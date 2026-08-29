using UnityEngine;

public class VideoPreloader : MonoBehaviour
{
    [SerializeField] private VideoCacheManager videoCacheManager;
    [SerializeField] private PlaybackManager playbackManager;
    [SerializeField] private VideoEntry antarcticVideo;

    private void Start()
    {
        if (videoCacheManager == null ||
            playbackManager == null ||
            antarcticVideo == null)
        {
            Debug.LogError(
                "VideoPreloader: Missing reference.");
            return;
        }

        string folder = Application.persistentDataPath;

        Debug.Log("VideoPreloader: Files currently in persistentDataPath:");

        foreach (string file in System.IO.Directory.GetFiles(folder))
        {
            Debug.Log("FOUND FILE: " + file);
        }

        Debug.Log(
            "VideoPreloader: persistentDataPath = " +
            Application.persistentDataPath);

        if (videoCacheManager.HasCachedVideo())
        {
            Debug.Log(
                "VideoPreloader: Cached video found.");

            playbackManager.LoadLocalFile(
                antarcticVideo,
                videoCacheManager.LocalVideoPath);
        }
        else
        {
            Debug.LogWarning(
                "VideoPreloader: Cached video NOT found at " +
                videoCacheManager.LocalVideoPath);
        }
    }
}