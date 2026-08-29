using UnityEngine;
using Ubiq.Samples;

public class AntarcticaLeaveRoomButton : MonoBehaviour
{
    [SerializeField] private SocialMenu mainMenu;

    public void LeaveRoom()
    {
        if (mainMenu == null)
        {
            Debug.LogError(
                "AntarcticaLeaveRoomButton: SocialMenu missing.");
            return;
        }

        if (HostManager.Instance != null)
        {
            HostManager.Instance.ClearRole();
        }

        Debug.Log(
            "AntarcticaLeaveRoomButton: Leaving current room.");

        mainMenu.roomClient.Join("", false);
    }
}