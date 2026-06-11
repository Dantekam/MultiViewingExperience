using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class UIManager : MonoBehaviour
{
    [Header("Role")]
    [SerializeField] private bool isProfessorMode = true;

    [Header("Skyboxes")]
    [SerializeField] private Material lobbySkybox;
    [SerializeField] private Material videoSkybox;

    [Header("References")]
    [SerializeField] private PlaybackManager playbackManager;

    [Header("Video")]
    [SerializeField] private VideoEntry antarcticVideo;

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject controlPanel;

    [Header("Professor Controls")]
    [SerializeField] private Slider timelineSlider;

    private bool ignoreSliderCallback = false;

    private bool controlsVisible = true;

    [SerializeField] private float toggleCooldown = 0.2f;
    private float nextToggleTime = 0f;

    private void Awake()
    {
        SetSkybox(lobbySkybox);

        if (startPanel != null)
            startPanel.SetActive(true);

        if (controlPanel != null)
            controlPanel.SetActive(false);

        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0f;
            timelineSlider.maxValue = 1f;
            timelineSlider.wholeNumbers = false;

            timelineSlider.onValueChanged.AddListener(OnTimelineValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (timelineSlider != null)
            timelineSlider.onValueChanged.RemoveListener(OnTimelineValueChanged);
    }

    private void Update()
    {
        UpdateTimelineSlider();
        HandleProfessorToggle();
    }

    private void HandleProfessorToggle()
    {
        if (!isProfessorMode)
            return;

        InputDevice leftController =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (!leftController.isValid)
            return;

        bool xPressed;

        if (leftController.TryGetFeatureValue(
            CommonUsages.primaryButton,
            out xPressed))
        {
            if (xPressed && Time.time >= nextToggleTime)
            {
                ToggleProfessorPanel();
                nextToggleTime = Time.time + toggleCooldown;
            }
        }
    }

    private void ToggleProfessorPanel()
    {
        if (controlPanel == null)
            return;

        controlsVisible = !controlsVisible;
        controlPanel.SetActive(controlsVisible);
    }

    public void OnPlayPressed()
    {
        Debug.Log("Play Pressed");

        if (startPanel != null)
            startPanel.SetActive(false);

        if (controlPanel != null)
            controlPanel.SetActive(isProfessorMode);

        SetSkybox(videoSkybox);

        if (playbackManager == null)
        {
            Debug.LogError("PlaybackManager missing.");
            return;
        }

        if (antarcticVideo == null)
        {
            Debug.LogError("Antarctic video missing.");
            return;
        }

        playbackManager.LoadVideo(antarcticVideo);
    }

    public void OnPausePressed()
    {
        if (!isProfessorMode)
            return;

        playbackManager?.PauseVideo();
    }

    public void OnResumePressed()
    {
        if (!isProfessorMode)
            return;

        playbackManager?.PlayVideo();
    }

    public void OnRestartPressed()
    {
        if (!isProfessorMode)
            return;

        if (playbackManager == null)
            return;

        playbackManager.SeekTo(0);
        playbackManager.PlayVideo();
    }

    private void UpdateTimelineSlider()
    {
        if (!isProfessorMode)
            return;

        if (timelineSlider == null)
            return;

        if (playbackManager == null)
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

        if (playbackManager == null)
            return;

        double length = playbackManager.CurrentLength;

        if (length <= 0)
            return;

        double targetTime = value * length;

        Debug.Log($"Seeking To {targetTime:F2}");

        playbackManager.SeekTo(targetTime);
    }

    private void SetSkybox(Material skybox)
    {
        if (skybox == null)
            return;

        RenderSettings.skybox = skybox;
        DynamicGI.UpdateEnvironment();
    }
}