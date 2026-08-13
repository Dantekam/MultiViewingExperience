using System;
using UnityEngine;
using Ubiq.Rooms;

public class HostManager : MonoBehaviour
{
    public static HostManager Instance { get; private set; }

    [Header("Ubiq")]
    [SerializeField] private RoomClient roomClient;

    public bool IsHost { get; private set; }
    public bool IsClient { get; private set; }
    public bool HasRole => IsHost || IsClient;

    public event Action OnRoleChanged;

    private enum PendingRole
    {
        None,
        Host,
        Client
    }

    private PendingRole pendingRole = PendingRole.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        IsHost = false;
        IsClient = false;
    }

    private void OnEnable()
    {
        if (roomClient != null)
        {
            roomClient.OnJoinedRoom.AddListener(OnJoinedRoom);
            roomClient.OnJoinRejected.AddListener(OnJoinRejected);
        }
    }

    private void OnDisable()
    {
        if (roomClient != null)
        {
            roomClient.OnJoinedRoom.RemoveListener(OnJoinedRoom);
            roomClient.OnJoinRejected.RemoveListener(OnJoinRejected);
        }
    }

    // Call this BEFORE creating a room.
    public void PrepareHostJoin()
    {
        pendingRole = PendingRole.Host;

        Debug.Log("HostManager: Waiting for HOST room creation...");
    }

    // Call this BEFORE joining somebody else's room.
    public void PrepareClientJoin()
    {
        pendingRole = PendingRole.Client;

        Debug.Log("HostManager: Waiting to join as CLIENT...");
    }

    private void OnJoinedRoom(IRoom room)
    {
        // Ignore Ubiq's empty room.
        if (room == null || string.IsNullOrEmpty(room.UUID))
        {
            return;
        }

        if (pendingRole == PendingRole.Host)
        {
            BecomeHost();
        }
        else if (pendingRole == PendingRole.Client)
        {
            BecomeClient();
        }

        pendingRole = PendingRole.None;
    }

    private void OnJoinRejected(Rejection rejection)
    {
        Debug.LogWarning(
            $"HostManager: Room join rejected: {rejection.reason}");

        pendingRole = PendingRole.None;
    }

    private void BecomeHost()
    {
        IsHost = true;
        IsClient = false;

        Debug.Log("====================================");
        Debug.Log("HostManager: THIS PLAYER IS HOST");
        Debug.Log("====================================");

        OnRoleChanged?.Invoke();
    }

    private void BecomeClient()
    {
        IsHost = false;
        IsClient = true;

        Debug.Log("====================================");
        Debug.Log("HostManager: THIS PLAYER IS CLIENT");
        Debug.Log("====================================");

        OnRoleChanged?.Invoke();
    }
}