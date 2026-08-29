using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private HostManager hostManager;
    [SerializeField] private EnvironmentManager environmentManager;
    [SerializeField] private VideoNetworkManager videoNetworkManager;
    [SerializeField] private PlaybackManager playbackManager;

    [Header("Audio")]
    [SerializeField] private AudioSource lobbyMusic;

    [Header("Skyboxes")]
    [SerializeField] private Material lobbySkybox;
    [SerializeField] private Material videoSkybox;

    [Header("Host Only Menu Buttons")]
    [SerializeField] private GameObject mainPlayButton;
    [SerializeField] private GameObject controlsButton;

    [Header("Professor Panel")]
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Slider timelineSlider;

    private bool ignoreSliderCallback = false;

    private void Awake()
    {
        SetSkybox(lobbySkybox);

        if (controlPanel != null)
            controlPanel.SetActive(false);

        if (mainPlayButton != null)
            mainPlayButton.SetActive(false);

        if (controlsButton != null)
            controlsButton.SetActive(false);

        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0f;
            timelineSlider.maxValue = 1f;
            timelineSlider.wholeNumbers = false;

            timelineSlider.onValueChanged.AddListener(
                OnTimelineValueChanged);
        }
    }

    private void Start()
    {
        RefreshHostUI();

        if (hostManager != null)
        {
            hostManager.OnRoleChanged += RefreshHostUI;
        }
    }

    private void OnDestroy()
    {
        if (timelineSlider != null)
        {
            timelineSlider.onValueChanged.RemoveListener(
                OnTimelineValueChanged);
        }

        if (hostManager != null)
        {
            hostManager.OnRoleChanged -= RefreshHostUI;
        }
    }

    private void Update()
    {
        UpdateTimelineSlider();
    }


    // Host UI
    public void RefreshHostUI()
    {
        bool host =
            hostManager != null &&
            hostManager.IsHost;

        Debug.Log(
            $"UIManager: RefreshHostUI - Host = {host}");

        if (mainPlayButton != null)
            mainPlayButton.SetActive(host);

        if (controlsButton != null)
            controlsButton.SetActive(host);

        if (!host && controlPanel != null)
            controlPanel.SetActive(false);
    }

    // Called locally AND on network clients.
    public void ApplyExperienceStarted()
    {
        Debug.Log("UIManager: Applying Experience Started");

        if (lobbyMusic != null)
            lobbyMusic.Stop();

        if (environmentManager != null)
            environmentManager.StartExperience();

        SetSkybox(videoSkybox);
    }


    // Professor Buttons

    public void OnPlayPressed()
    {
        if (!CanControlVideo())
            return;

        Debug.Log("UIManager: Host pressed START");

        videoNetworkManager?.SendPlay();
    }

    public void OnPausePressed()
    {
        if (!CanControlVideo())
            return;

        Debug.Log("UIManager: Host pressed PAUSE");

        videoNetworkManager?.SendPause();
    }

    public void OnResumePressed()
    {
        if (!CanControlVideo())
            return;

        Debug.Log("UIManager: Host pressed RESUME");

        videoNetworkManager?.SendResume();
    }

    public void OnRestartPressed()
    {
        if (!CanControlVideo())
            return;

        Debug.Log("UIManager: Host pressed RESTART");

        videoNetworkManager?.SendRestart();
    }

    // Timeline
    private void UpdateTimelineSlider()
    {
        if (!CanControlVideo())
            return;

        if (timelineSlider == null ||
            playbackManager == null)
            return;

        double length = playbackManager.CurrentLength;

        if (length <= 0)
            return;

        float normalized =
            (float)(playbackManager.CurrentTime / length);

        ignoreSliderCallback = true;

        timelineSlider.SetValueWithoutNotify(normalized);

        ignoreSliderCallback = false;
    }

    private void OnTimelineValueChanged(float value)
    {
        if (ignoreSliderCallback)
            return;

        if (!CanControlVideo())
            return;

        if (playbackManager == null ||
            videoNetworkManager == null)
            return;

        double length = playbackManager.CurrentLength;

        if (length <= 0)
            return;

        double targetTime = value * length;

        videoNetworkManager.SendSeek(targetTime);
    }

    private bool CanControlVideo()
    {
        return hostManager != null &&
               hostManager.IsHost;
    }

    // Skybox
    private void SetSkybox(Material skybox)
    {
        if (skybox == null)
            return;

        RenderSettings.skybox = skybox;
        DynamicGI.UpdateEnvironment();
    }

    public void ReturnToLobby()
    {
        Debug.Log("UIManager: Returning to lobby.");

        if (playbackManager != null)
        {
            playbackManager.StopVideo();
        }

        if (environmentManager != null)
        {
            environmentManager.ShowEnvironment();
        }

        SetSkybox(lobbySkybox);

        if (lobbyMusic != null)
        {
            if (!lobbyMusic.isPlaying)
            {
                lobbyMusic.Play();
            }
        }

        if (controlPanel != null)
        {
            controlPanel.SetActive(false);
        }

        RefreshHostUI();
    }
}