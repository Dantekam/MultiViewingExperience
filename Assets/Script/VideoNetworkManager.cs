
using System;
using UnityEngine;
using Ubiq.Messaging;

public class VideoNetworkManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlaybackManager playbackManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private VideoCacheManager videoCacheManager;
    [SerializeField] private VideoLibraryManager videoLibraryManager;

    [Header("Video")]
    [SerializeField] private VideoEntry antarcticVideo;

    private NetworkContext context;
    private bool applyingNetworkSelection;

    [Serializable]
    private struct VideoMessage
    {
        public string command;
        public double time;
        public int videoIndex;
    }

    private void OnEnable()
    {
        if (videoLibraryManager != null)
        {
            videoLibraryManager.VideoSelectionChanged +=
                OnVideoSelectionChanged;
        }
    }

    private void OnDisable()
    {
        if (videoLibraryManager != null)
        {
            videoLibraryManager.VideoSelectionChanged -=
                OnVideoSelectionChanged;
        }
    }

    private void Start()
    {
        context = NetworkScene.Register(this);

        Debug.Log("VideoNetworkManager: Registered with Ubiq.");
    }

    // =====================================================
    // VIDEO SELECTION
    // =====================================================

    private void OnVideoSelectionChanged(
        int index,
        VideoEntry entry)
    {
        if (applyingNetworkSelection)
            return;

        if (!CanSend())
            return;

        VideoMessage message = new VideoMessage
        {
            command = "SelectVideo",
            videoIndex = index,
            time = 0
        };

        Debug.Log(
            $"VideoNetworkManager: Broadcasting selection {index}.");

        SendMessageToClients(message);
    }

    private void ApplyVideoSelection(int index)
    {
        if (videoLibraryManager == null)
        {
            Debug.LogError(
                "VideoNetworkManager: VideoLibraryManager missing.");
            return;
        }

        if (index < 0 || index >= videoLibraryManager.VideoCount)
        {
            Debug.LogWarning(
                $"VideoNetworkManager: Invalid video index {index}.");
            return;
        }

        applyingNetworkSelection = true;

        try
        {
            videoLibraryManager.SelectVideo(index);
        }
        finally
        {
            applyingNetworkSelection = false;
        }

        Debug.Log(
            $"VideoNetworkManager: Applied video selection {index}.");
    }

    // =====================================================
    // HOST COMMANDS
    // =====================================================

    public void SendPlay()
    {
        if (!CanSend())
            return;

        SendAndApply(new VideoMessage
        {
            command = "Start",
            time = 0
        });
    }

    public void SendResume()
    {
        if (!CanSend())
            return;

        SendAndApply(new VideoMessage
        {
            command = "Resume",
            time = playbackManager != null
                ? playbackManager.CurrentTime
                : 0
        });
    }

    public void SendPause()
    {
        if (!CanSend())
            return;

        SendAndApply(new VideoMessage
        {
            command = "Pause",
            time = playbackManager != null
                ? playbackManager.CurrentTime
                : 0
        });
    }

    public void SendRestart()
    {
        if (!CanSend())
            return;

        SendAndApply(new VideoMessage
        {
            command = "Restart",
            time = 0
        });
    }

    public void SendSeek(double time)
    {
        if (!CanSend())
            return;

        SendAndApply(new VideoMessage
        {
            command = "Seek",
            time = time
        });
    }

    // =====================================================
    // NETWORK
    // =====================================================

    private void SendAndApply(VideoMessage message)
    {
        ApplyMessage(message);
        SendMessageToClients(message);
    }

    private void SendMessageToClients(VideoMessage message)
    {
        if (context.Scene != null)
        {
            Debug.Log(
                $"VideoNetworkManager: Sending {message.command}");

            context.SendJson(message);
        }
        else
        {
            Debug.LogWarning(
                "VideoNetworkManager: Network Scene unavailable.");
        }
    }

    public void ProcessMessage(
        ReferenceCountedSceneGraphMessage networkMessage)
    {
        VideoMessage message =
            networkMessage.FromJson<VideoMessage>();

        Debug.Log(
            $"VideoNetworkManager: Received {message.command}");

        ApplyMessage(message);
    }

    // =====================================================
    // APPLY COMMAND LOCALLY
    // =====================================================

    private void ApplyMessage(VideoMessage message)
    {
        if (message.command == "SelectVideo")
        {
            ApplyVideoSelection(message.videoIndex);
            return;
        }

        if (playbackManager == null)
        {
            Debug.LogError(
                "VideoNetworkManager: PlaybackManager missing.");
            return;
        }

        switch (message.command)
        {
            case "Start":
            {
                if (!playbackManager.IsPrepared)
                {
                    Debug.LogWarning(
                        "VideoNetworkManager: Cannot start - video not prepared.");
                    break;
                }

                playbackManager.SeekTo(0);
                playbackManager.PlayVideo();

                if (uiManager != null)
                    uiManager.ApplyExperienceStarted();

                break;
            }

            case "Resume":
            {
                if (!playbackManager.IsPrepared)
                    break;

                playbackManager.PlayVideo();
                break;
            }

            case "Pause":
            {
                playbackManager.PauseVideo();
                break;
            }

            case "Restart":
            {
                if (!playbackManager.IsPrepared)
                    break;

                playbackManager.SeekTo(0);
                playbackManager.PlayVideo();
                break;
            }

            case "Seek":
            {
                if (!playbackManager.IsPrepared)
                    break;

                playbackManager.SeekTo(message.time);
                break;
            }
        }
    }

    private bool CanSend()
    {
        return HostManager.Instance != null &&
               HostManager.Instance.IsHost;
    }
}
