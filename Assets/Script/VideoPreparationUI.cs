using UnityEngine;
using UnityEngine.UI;

public class VideoPreparationUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlaybackManager playbackManager;
    [SerializeField] private VideoLibraryManager videoLibraryManager;

    [Header("UI")]
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject preparingIndicator;
    [SerializeField] private GameObject readyIndicator;

    private void OnEnable()
    {
        if (playbackManager != null)
        {
            playbackManager.VideoPrepareStarted += OnPrepareStarted;
            playbackManager.VideoPrepared += OnPrepared;
            playbackManager.VideoError += OnVideoError;
        }

        RefreshCurrentState();
    }

    private void OnDisable()
    {
        if (playbackManager != null)
        {
            playbackManager.VideoPrepareStarted -= OnPrepareStarted;
            playbackManager.VideoPrepared -= OnPrepared;
            playbackManager.VideoError -= OnVideoError;
        }
    }

    private void OnPrepareStarted(VideoEntry video)
    {
        if (video == null)
            return;

        SetPreparing(video);
    }

    private void OnPrepared(VideoEntry video)
    {
        if (video == null)
            return;

        // Ignore an old prepare event if another video
        // has since been selected.
        if (videoLibraryManager != null &&
            videoLibraryManager.SelectedVideo != video)
        {
            return;
        }

        SetReady(video);
    }

    private void OnVideoError(VideoEntry video, string message)
    {
        if (videoLibraryManager != null &&
            videoLibraryManager.SelectedVideo != video)
        {
            return;
        }

        if (statusText != null)
        {
            statusText.text = "Video Error";
        }

        if (preparingIndicator != null)
        {
            preparingIndicator.SetActive(false);
        }

        if (readyIndicator != null)
        {
            readyIndicator.SetActive(false);
        }
    }

    public void RefreshCurrentState()
    {
        if (videoLibraryManager == null ||
            videoLibraryManager.SelectedVideo == null)
        {
            if (statusText != null)
            {
                statusText.text = "No Video Selected";
            }

            if (preparingIndicator != null)
            {
                preparingIndicator.SetActive(false);
            }

            if (readyIndicator != null)
            {
                readyIndicator.SetActive(false);
            }

            return;
        }

        if (videoLibraryManager.IsSelectedVideoPrepared)
        {
            SetReady(videoLibraryManager.SelectedVideo);
        }
        else
        {
            SetPreparing(videoLibraryManager.SelectedVideo);
        }
    }

    private void SetPreparing(VideoEntry video)
    {
        if (statusText != null)
        {
            statusText.text =
                $"{video.title} - Preparing...";
        }

        if (preparingIndicator != null)
        {
            preparingIndicator.SetActive(true);
        }

        if (readyIndicator != null)
        {
            readyIndicator.SetActive(false);
        }

        Debug.Log(
            $"VideoPreparationUI: {video.title} is PREPARING.");
    }

    private void SetReady(VideoEntry video)
    {
        if (statusText != null)
        {
            statusText.text =
                $"{video.title} - Ready";
        }

        if (preparingIndicator != null)
        {
            preparingIndicator.SetActive(false);
        }

        if (readyIndicator != null)
        {
            readyIndicator.SetActive(true);
        }

        Debug.Log(
            $"VideoPreparationUI: {video.title} is READY.");
    }
}