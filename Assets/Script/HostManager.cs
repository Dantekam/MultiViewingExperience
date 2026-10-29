
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

    public const string RoleProperty = "experience.role";

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

    public void PrepareHostJoin()
    {
        pendingRole = PendingRole.Host;

        Debug.Log(
            "HostManager: Waiting for HOST room creation...");
    }

    public void PrepareClientJoin()
    {
        pendingRole = PendingRole.Client;

        Debug.Log(
            "HostManager: Waiting to join as CLIENT...");
    }

    private void OnJoinedRoom(IRoom room)
    {
        if (room == null || string.IsNullOrEmpty(room.UUID))
            return;

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

        PublishRole("host");

        Debug.Log("====================================");
        Debug.Log("HostManager: THIS PLAYER IS HOST");
        Debug.Log("====================================");

        OnRoleChanged?.Invoke();
    }

    private void BecomeClient()
    {
        IsHost = false;
        IsClient = true;

        PublishRole("client");

        Debug.Log("====================================");
        Debug.Log("HostManager: THIS PLAYER IS CLIENT");
        Debug.Log("====================================");

        OnRoleChanged?.Invoke();
    }

    private void PublishRole(string role)
    {
        if (roomClient == null || roomClient.Me == null)
        {
            Debug.LogWarning(
                "HostManager: Cannot publish role - local peer missing.");

            return;
        }

        roomClient.Me[RoleProperty] = role;

        Debug.Log(
            $"HostManager: Published role '{role}'.");
    }

    public void ClearRole()
    {
        PublishRole("");

        IsHost = false;
        IsClient = false;
        pendingRole = PendingRole.None;

        Debug.Log(
            "HostManager: Role cleared - NOT IN ROOM");

        OnRoleChanged?.Invoke();
    }
}
