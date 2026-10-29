using UnityEngine;
using Ubiq.Samples;

public class AntarcticaLeaveRoomButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SocialMenu mainMenu;
    [SerializeField] private UIManager uiManager;

    public void LeaveRoom()
    {
        Debug.Log(
            "AntarcticaLeaveRoomButton: Leaving shared experience.");

        if (HostManager.Instance != null)
        {
            HostManager.Instance.ClearRole();
        }

        if (uiManager != null)
        {
            uiManager.ReturnToLobby();
        }
    }
}