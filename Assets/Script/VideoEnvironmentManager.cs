using UnityEngine;

public class VideoEnvironmentManager : MonoBehaviour
{
    [Header("Video Environments")]
    [SerializeField] private GameObject snowyEnvironment;
    [SerializeField] private GameObject swampEnvironment;
    [SerializeField] private GameObject lakeEnvironment;

    public void SetEnvironment(int videoIndex)
    {
        // Turn everything off first.
        SetAllEnvironments(false);

        switch (videoIndex)
        {
            // Antarctica
            case 0:
                if (snowyEnvironment != null)
                    snowyEnvironment.SetActive(true);

                Debug.Log(
                    "VideoEnvironmentManager: Snowy Environment enabled.");
                break;

            // Lake
            case 1:
                if (lakeEnvironment != null)
                    lakeEnvironment.SetActive(true);

                Debug.Log(
                    "VideoEnvironmentManager: Lake Environment enabled.");
                break;

            // Swamp
            case 2:
                if (swampEnvironment != null)
                    swampEnvironment.SetActive(true);

                Debug.Log(
                    "VideoEnvironmentManager: Swamp Environment enabled.");
                break;

            default:
                Debug.LogWarning(
                    $"VideoEnvironmentManager: No environment configured for video index {videoIndex}.");
                break;
        }
    }

    private void SetAllEnvironments(bool active)
    {
        if (snowyEnvironment != null)
            snowyEnvironment.SetActive(active);

        if (swampEnvironment != null)
            swampEnvironment.SetActive(active);

        if (lakeEnvironment != null)
            lakeEnvironment.SetActive(active);
    }
}