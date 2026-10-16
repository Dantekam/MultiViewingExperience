
using UnityEngine;

public class ExperienceVisibilityManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HostManager hostManager;
    [SerializeField] private PlaybackManager playbackManager;

    [Header("Student UI")]
    [SerializeField] private GameObject studentMenuRoot;

    private bool experienceActive;

    private void OnEnable()
    {
        if (hostManager != null)
            hostManager.OnRoleChanged += RefreshVisibility;

        if (playbackManager != null)
        {
            playbackManager.VideoStarted += OnVideoStarted;
            playbackManager.VideoFinished += OnVideoFinished;
            playbackManager.VideoError += OnVideoError;
        }
    }

    private void Start()
    {
        RefreshVisibility();
    }

    private void OnDisable()
    {
        if (hostManager != null)
            hostManager.OnRoleChanged -= RefreshVisibility;

        if (playbackManager != null)
        {
            playbackManager.VideoStarted -= OnVideoStarted;
            playbackManager.VideoFinished -= OnVideoFinished;
            playbackManager.VideoError -= OnVideoError;
        }
    }

    private void OnVideoStarted(VideoEntry entry)
    {
        SetExperienceActive(true);
    }

    private void OnVideoFinished(VideoEntry entry)
    {
        SetExperienceActive(false);
    }

    private void OnVideoError(VideoEntry entry, string message)
    {
        SetExperienceActive(false);
    }

    public void SetExperienceActive(bool active)
    {
        experienceActive = active;
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        if (hostManager == null || studentMenuRoot == null)
            return;

        bool hideMenu =
            experienceActive && hostManager.IsClient;

        studentMenuRoot.SetActive(!hideMenu);
    }
}
