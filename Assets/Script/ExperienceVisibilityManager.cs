using UnityEngine;
using Ubiq.Avatars;
using Avatar = Ubiq.Avatars.Avatar;

public class ExperienceVisibilityManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HostManager hostManager;
    [SerializeField] private PlaybackManager playbackManager;
    [SerializeField] private AvatarManager avatarManager;

    [Header("Student UI")]
    [SerializeField] private GameObject studentMenuRoot;

    [Header("Avatar Refresh")]
    [SerializeField] private float refreshInterval = 0.5f;

    private bool experienceActive;
    private float nextRefreshTime;

    private void OnEnable()
    {
        if (hostManager != null)
        {
            hostManager.OnRoleChanged += RefreshVisibility;
        }

        if (playbackManager != null)
        {
            playbackManager.VideoStarted += OnVideoStarted;
            playbackManager.VideoFinished += OnVideoFinished;
            playbackManager.VideoError += OnVideoError;
        }
    }

    private void Start()
    {
        if (avatarManager == null)
        {
            avatarManager =
                FindObjectOfType<AvatarManager>();
        }

        RefreshVisibility();
    }

    private void Update()
    {
        if (Time.time < nextRefreshTime)
            return;

        nextRefreshTime =
            Time.time + Mathf.Max(0.1f, refreshInterval);

        RefreshAvatars();
    }

    private void OnDisable()
    {
        if (hostManager != null)
        {
            hostManager.OnRoleChanged -= RefreshVisibility;
        }

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

    private void OnVideoError(
        VideoEntry entry,
        string message)
    {
        SetExperienceActive(false);
    }

    public void SetExperienceActive(bool active)
    {
        experienceActive = active;

        Debug.Log(
            $"ExperienceVisibilityManager: Experience active = {active}");

        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        RefreshMenu();
        RefreshAvatars();
    }

    private void RefreshMenu()
    {
        if (hostManager == null ||
            studentMenuRoot == null)
        {
            return;
        }

        bool hideMenu =
            experienceActive &&
            hostManager.IsClient;

        studentMenuRoot.SetActive(!hideMenu);
    }

    private void RefreshAvatars()
    {
        if (avatarManager == null)
            return;

        foreach (Avatar avatar in avatarManager.Avatars)
        {
            if (avatar == null || avatar.Peer == null)
                continue;

            StudentAvatarVisibility visibility =
                avatar.GetComponent<StudentAvatarVisibility>();

            if (visibility == null)
                continue;

            string role =
                avatar.Peer[HostManager.RoleProperty];

            bool isStudent = role == "client";

            bool shouldBeVisible =
                !experienceActive || !isStudent;

            visibility.SetVisible(shouldBeVisible);
        }
    }
}
