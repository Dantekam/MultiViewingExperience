using UnityEngine;
using UnityEngine.UI;
using Ubiq.Samples;

public class ClientJoinButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SocialMenu mainMenu;
    [SerializeField] private Text joinCodeText;

    public void JoinRoom()
    {
        if (mainMenu == null)
        {
            Debug.LogError(
                "ClientJoinButton: Main Menu / SocialMenu is missing.");

            return;
        }

        if (joinCodeText == null)
        {
            Debug.LogError(
                "ClientJoinButton: Join Code Text is missing.");

            return;
        }

        if (HostManager.Instance == null)
        {
            Debug.LogError(
                "ClientJoinButton: HostManager is missing.");

            return;
        }

        string joinCode =
            joinCodeText.text.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning(
                "ClientJoinButton: Join code is empty.");

            return;
        }

        Debug.Log(
            $"ClientJoinButton: Attempting to join {joinCode} as CLIENT.");

        // Tell HostManager what role we should receive
        // IF Ubiq successfully joins the room.
        HostManager.Instance.PrepareClientJoin();

        // Let Ubiq perform the actual room join.
        mainMenu.roomClient.Join(
            joincode: joinCode);
    }
}