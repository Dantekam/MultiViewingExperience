using System;
using UnityEngine;

public class VideoLibraryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoCacheManager videoCacheManager;
    [SerializeField] private PlaybackManager playbackManager;
    [SerializeField] private VideoEnvironmentManager videoEnvironmentManager;

    [Header("Available Videos")]
    [SerializeField] private VideoEntry[] videos;

    [Header("Selection Visuals")]
    [SerializeField] private VideoSelectionVisual[] selectionVisuals;

    [Header("Startup")]
    [SerializeField] private int defaultVideoIndex = 0;

    public VideoEntry SelectedVideo { get; private set; }
    public int SelectedVideoIndex { get; private set; } = -1;

    public bool IsSelectedVideoPrepared
    {
        get
        {
            return playbackManager != null &&
                   playbackManager.IsPrepared &&
                   playbackManager.CurrentVideo == SelectedVideo;
        }
    }

    public event Action<int, VideoEntry> VideoSelectionChanged;

    private void Start()
    {
        Debug.Log("==============================");
        Debug.Log("VideoLibraryManager: START");

        if (videoCacheManager == null)
        {
            Debug.LogError(
                "VideoLibraryManager: VideoCacheManager is missing.");
            return;
        }

        if (playbackManager == null)
        {
            Debug.LogError(
                "VideoLibraryManager: PlaybackManager is missing.");
            return;
        }

        if (videos == null || videos.Length == 0)
        {
            Debug.LogError(
                "VideoLibraryManager: No videos configured.");
            return;
        }

        Debug.Log(
            $"VideoLibraryManager: {videos.Length} videos configured.");

        for (int i = 0; i < videos.Length; i++)
        {
            if (videos[i] == null)
            {
                Debug.LogWarning(
                    $"VideoLibraryManager: Video index {i} is NULL.");
                continue;
            }

            Debug.Log(
                $"VideoLibraryManager: [{i}] " +
                $"title='{videos[i].title}', " +
                $"file='{videos[i].fileName}'");
        }

        if (defaultVideoIndex < 0 ||
            defaultVideoIndex >= videos.Length)
        {
            defaultVideoIndex = 0;
        }

        // This should happen ONCE when the application starts.
        // Room creation, joining, leaving, and opening/closing Browse
        // should NOT call this again.
        Debug.Log(
            $"VideoLibraryManager: Loading default video index {defaultVideoIndex}.");

        SelectVideo(defaultVideoIndex);

        Debug.Log("==============================");
    }

    // =========================================================
    // VIDEO SELECTION
    // =========================================================

    public void SelectVideo(int index)
    {
        Debug.Log("==============================");
        Debug.Log(
            $"VIDEO BUTTON CLICK RECEIVED: index = {index}");

        if (videos == null)
        {
            Debug.LogError(
                "VideoLibraryManager: Videos array is null.");
            return;
        }

        if (index < 0 || index >= videos.Length)
        {
            Debug.LogError(
                $"VideoLibraryManager: Invalid video index {index}.");
            return;
        }

        VideoEntry entry = videos[index];

        if (entry == null)
        {
            Debug.LogError(
                $"VideoLibraryManager: Video at index {index} is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(entry.fileName))
        {
            Debug.LogError(
                $"VideoLibraryManager: '{entry.title}' has no fileName.");
            return;
        }

        // Store the selection BEFORE preparing.
        // This means the selection remains true even while the
        // VideoPlayer is still preparing the file.
        SelectedVideoIndex = index;
        SelectedVideo = entry;

        Debug.Log(
            $"VIDEO SELECTED: index={index}, " +
            $"title='{entry.title}', " +
            $"file='{entry.fileName}'");

        // Immediately update the surrounding environment.
        // This also provides visual confirmation that the
        // user's video selection was received.
        UpdateEnvironment();

        // Update the Browse menu selection highlight.
        UpdateSelectionVisuals();

        // Notify any other systems interested in video selection.
        VideoSelectionChanged?.Invoke(
            SelectedVideoIndex,
            SelectedVideo);

        // Begin preparing the selected video.
        PrepareSelectedVideo();

        Debug.Log("==============================");
    }

    // =========================================================
    // ENVIRONMENT
    // =========================================================

    private void UpdateEnvironment()
    {
        if (videoEnvironmentManager == null)
        {
            Debug.LogWarning(
                "VideoLibraryManager: VideoEnvironmentManager is not assigned.");

            return;
        }

        videoEnvironmentManager.SetEnvironment(
            SelectedVideoIndex);

        Debug.Log(
            $"VideoLibraryManager: Environment updated for " +
            $"video index {SelectedVideoIndex}.");
    }

    // =========================================================
    // PREPARATION
    // =========================================================

    private void PrepareSelectedVideo()
    {
        if (SelectedVideo == null)
        {
            Debug.LogError(
                "VideoLibraryManager: No video is currently selected.");
            return;
        }

        string fileName = SelectedVideo.fileName;

        string localPath =
            videoCacheManager.GetLocalVideoPath(fileName);

        Debug.Log(
            $"VideoLibraryManager: Checking for '{fileName}'");

        Debug.Log(
            $"VideoLibraryManager: Expected path = {localPath}");

        if (!videoCacheManager.HasCachedVideo(fileName))
        {
            Debug.LogError(
                $"VIDEO FILE NOT FOUND: {localPath}");

            return;
        }

        Debug.Log(
            $"VIDEO FILE FOUND: {localPath}");

        Debug.Log(
            $"VideoLibraryManager: Preparing {SelectedVideo.title}");

        playbackManager.LoadLocalFile(
            SelectedVideo,
            localPath);
    }

    // =========================================================
    // SELECTION VISUALS
    // =========================================================

    public void RefreshSelectionVisuals()
    {
        UpdateSelectionVisuals();
    }

    private void UpdateSelectionVisuals()
    {
        if (selectionVisuals == null)
            return;

        for (int i = 0; i < selectionVisuals.Length; i++)
        {
            if (selectionVisuals[i] == null)
                continue;

            bool isSelected =
                i == SelectedVideoIndex;

            selectionVisuals[i].SetSelected(
                isSelected);
        }

        Debug.Log(
            $"VideoLibraryManager: Selection visuals refreshed. " +
            $"Selected index = {SelectedVideoIndex}");
    }

    // =========================================================
    // ACCESS
    // =========================================================

    public VideoEntry GetVideo(int index)
    {
        if (videos == null ||
            index < 0 ||
            index >= videos.Length)
        {
            return null;
        }

        return videos[index];
    }

    public int VideoCount
    {
        get
        {
            return videos != null
                ? videos.Length
                : 0;
        }
    }
}