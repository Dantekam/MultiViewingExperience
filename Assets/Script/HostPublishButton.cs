using UnityEngine;
using Ubiq.Samples;

public class HostPublishButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SocialMenu mainMenu;

    [Header("Room Settings")]
    [SerializeField] private bool publish = true;

    public void PublishRoom()
    {
        if (mainMenu == null)
        {
            Debug.LogError(
                "HostPublishButton: Main Menu / SocialMenu is missing.");

            return;
        }

        if (HostManager.Instance == null)
        {
            Debug.LogError(
                "HostPublishButton: HostManager is missing.");

            return;
        }

        // Tell HostManager that if this join succeeds,
        // this player should become the host.
        HostManager.Instance.PrepareHostJoin();

        // We removed the room-name keyboard,
        // so automatically generate a simple random name.
        int randomNumber = Random.Range(1000, 10000);
        string roomName = $"Room {randomNumber}";

        Debug.Log(
            $"HostPublishButton: Creating {roomName} | Published: {publish}");

        mainMenu.roomClient.Join(
            name: roomName,
            publish: publish);
    }
}