using UnityEngine;
using Ubiq.Samples;

public class AntarcticaMenuNavigation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Ubiq.Samples.PanelSwitcher panelSwitcher;

    [Header("Panels")]
    [SerializeField] private GameObject newRoomPublishPanel;
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private GameObject professorControlPanel;

    public void OpenNewRoomPublishPanel()
    {
        if (panelSwitcher == null || newRoomPublishPanel == null)
        {
            Debug.LogError(
                "AntarcticaMenuNavigation: New Room references are missing.");
            return;
        }

        panelSwitcher.SwitchPanel(newRoomPublishPanel);
    }

    public void OpenJoinRoomPanel()
    {
        if (panelSwitcher == null || joinRoomPanel == null)
        {
            Debug.LogError(
                "AntarcticaMenuNavigation: Join Room references are missing.");
            return;
        }

        panelSwitcher.SwitchPanel(joinRoomPanel);
    }

    public void OpenProfessorControls()
    {
        if (panelSwitcher == null || professorControlPanel == null)
        {
            Debug.LogError(
                "AntarcticaMenuNavigation: Professor Controls references are missing.");
            return;
        }

        panelSwitcher.SwitchPanel(professorControlPanel);
    }

    public void GoBackToMainMenu()
    {
        if (panelSwitcher == null)
        {
            Debug.LogError(
                "AntarcticaMenuNavigation: PanelSwitcher is missing.");
            return;
        }

        panelSwitcher.SwitchPanelToDefault();
    }
}