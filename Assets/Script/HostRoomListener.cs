using UnityEngine;
using Ubiq.Rooms;

public class HostRoomListener : MonoBehaviour
{
    [SerializeField]
    private RoomClient roomClient;

    private void OnEnable()
    {
        roomClient.OnJoinedRoom.AddListener(OnJoinedRoom);
    }

    private void OnDisable()
    {
        roomClient.OnJoinedRoom.RemoveListener(OnJoinedRoom);
    }

    private void OnJoinedRoom(IRoom room)
    {
        // We'll determine host here
    }
}