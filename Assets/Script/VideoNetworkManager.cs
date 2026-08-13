using System;
using UnityEngine;
using Ubiq.Messaging;

public class VideoNetworkManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlaybackManager playbackManager;
    [SerializeField] private UIManager uiManager;

    [Header("Video")]
    [SerializeField] private VideoEntry antarcticVideo;

    private NetworkContext context;

    [Serializable]
    private struct VideoMessage
    {
        public string command;
        public double time;
    }

    private void Start()
    {
        context = NetworkScene.Register(this);

        Debug.Log(
            "VideoNetworkManager: Registered with Ubiq.");
    }

    // ==========================================
    // HOST SEND COMMANDS
    // ==========================================

    public void SendPlay()
    {
        if (!CanSend())
            return;

        VideoMessage message = new VideoMessage
        {
            command = "Start",
            time = 0
        };

        SendAndApply(message);
    }

    public void SendResume()
    {
        if (!CanSend())
            return;

        VideoMessage message = new VideoMessage
        {
            command = "Resume",
            time = playbackManager != null
                ? playbackManager.CurrentTime
                : 0
        };

        SendAndApply(message);
    }

    public void SendPause()
    {
        if (!CanSend())
            return;

        VideoMessage message = new VideoMessage
        {
            command = "Pause",
            time = playbackManager != null
                ? playbackManager.CurrentTime
                : 0
        };

        SendAndApply(message);
    }

    public void SendRestart()
    {
        if (!CanSend())
            return;

        VideoMessage message = new VideoMessage
        {
            command = "Restart",
            time = 0
        };

        SendAndApply(message);
    }

    public void SendSeek(double time)
    {
        if (!CanSend())
            return;

        VideoMessage message = new VideoMessage
        {
            command = "Seek",
            time = time
        };

        SendAndApply(message);
    }

    // ==========================================
    // NETWORK
    // ==========================================

    private void SendAndApply(VideoMessage message)
    {
        // Host performs the command locally.
        ApplyMessage(message);

        // Send it to the other users.
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

    // ==========================================
    // APPLY COMMAND LOCALLY
    // ==========================================

    private void ApplyMessage(VideoMessage message)
    {
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
                Debug.Log(
                    "VideoNetworkManager: Applying START.");

                if (uiManager != null)
                    uiManager.ApplyExperienceStarted();

                if (antarcticVideo != null)
                    playbackManager.LoadVideo(antarcticVideo);

                break;
            }

            case "Resume":
            {
                Debug.Log(
                    "VideoNetworkManager: Applying RESUME.");

                playbackManager.PlayVideo();

                break;
            }

            case "Pause":
            {
                Debug.Log(
                    "VideoNetworkManager: Applying PAUSE.");

                playbackManager.PauseVideo();

                break;
            }

            case "Restart":
            {
                Debug.Log(
                    "VideoNetworkManager: Applying RESTART.");

                playbackManager.SeekTo(0);
                playbackManager.PlayVideo();

                break;
            }

            case "Seek":
            {
                Debug.Log(
                    $"VideoNetworkManager: Applying SEEK {message.time:F2}");

                playbackManager.SeekTo(message.time);

                break;
            }
        }
    }

    private bool CanSend()
    {
        if (HostManager.Instance == null)
        {
            Debug.LogWarning(
                "VideoNetworkManager: HostManager missing.");

            return false;
        }

        if (!HostManager.Instance.IsHost)
        {
            Debug.LogWarning(
                "VideoNetworkManager: CLIENT cannot send video controls.");

            return false;
        }

        return true;
    }
}