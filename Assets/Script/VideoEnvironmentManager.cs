using UnityEngine;
using UnityEngine.UI;

public class VideoEnvironmentManager : MonoBehaviour
{
    [Header("Video Environments")]
    [SerializeField] private GameObject snowyEnvironment;
    [SerializeField] private GameObject swampEnvironment;
    [SerializeField] private GameObject lakeEnvironment;
    [SerializeField] private GameObject oceanEnvironment;

    [Header("Lobby Title")]
    [SerializeField] private Text environmentTitle;

    [Header("Playback")]
    [SerializeField] private PlaybackManager playbackManager;

    [Header("Startup")]
    [SerializeField, Range(0, 3)] private int initialVideoIndex = 0;

    private int selectedVideoIndex;
    private bool videoMode;

    private void Awake()
    {
        selectedVideoIndex = initialVideoIndex;
        videoMode = false;

        RefreshEnvironment();
    }

    private void OnEnable()
    {
        if (playbackManager == null)
            return;

        playbackManager.VideoStarted += OnVideoStarted;
        playbackManager.VideoFinished += OnVideoFinished;
        playbackManager.VideoError += OnVideoError;
    }

    private void OnDisable()
    {
        if (playbackManager == null)
            return;

        playbackManager.VideoStarted -= OnVideoStarted;
        playbackManager.VideoFinished -= OnVideoFinished;
        playbackManager.VideoError -= OnVideoError;
    }

    public void SetEnvironment(int videoIndex)
    {
        if (videoIndex < 0 || videoIndex > 3)
        {
            Debug.LogWarning(
                $"VideoEnvironmentManager: Invalid video index {videoIndex}.");
            return;
        }

        selectedVideoIndex = videoIndex;

        RefreshEnvironment();

        Debug.Log(
            $"VideoEnvironmentManager: Selected environment {videoIndex}.");
    }

    public void ShowVideoEnvironment()
    {
        videoMode = true;
        RefreshEnvironment();
    }

    public void ShowLobbyEnvironment()
    {
        videoMode = false;
        RefreshEnvironment();
    }

    private void OnVideoStarted(VideoEntry entry)
    {
        ShowVideoEnvironment();
    }

    private void OnVideoFinished(VideoEntry entry)
    {
        ShowLobbyEnvironment();
    }

    private void OnVideoError(VideoEntry entry, string message)
    {
        ShowLobbyEnvironment();
    }

    private void RefreshEnvironment()
    {
        SetAllEnvironments(false);

        if (!videoMode)
        {
            switch (selectedVideoIndex)
            {
                case 0:
                    SetActive(snowyEnvironment, true);
                    break;

                case 1:
                    SetActive(lakeEnvironment, true);
                    break;

                case 2:
                    SetActive(swampEnvironment, true);
                    break;

                case 3:
                    SetActive(oceanEnvironment, true);
                    break;
            }
        }

        if (environmentTitle != null)
        {
            switch (selectedVideoIndex)
            {
                case 0:
                    environmentTitle.text = "Antarctica";
                    break;

                case 1:
                    environmentTitle.text = "Lake";
                    break;

                case 2:
                    environmentTitle.text = "Swamp";
                    break;

                case 3:
                    environmentTitle.text = "Ocean";
                    break;
            }
        }
    }

    private void SetAllEnvironments(bool active)
    {
        SetActive(snowyEnvironment, active);
        SetActive(lakeEnvironment, active);
        SetActive(swampEnvironment, active);
        SetActive(oceanEnvironment, active);
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null && target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }
}