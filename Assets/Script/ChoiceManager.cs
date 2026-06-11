using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private PlaybackManager playbackManager;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;

    [SerializeField] private TMP_Text titleText;

    [SerializeField] private Button choiceButtonA;
    [SerializeField] private Button choiceButtonB;

    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;

    [Header("Temporary Demo Choices")]
    [SerializeField] private VideoEntry videoA;
    [SerializeField] private VideoEntry videoB;

    private void Start()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);

        playbackManager.VideoFinished += OnVideoFinished;
    }

    private void OnDestroy()
    {
        if (playbackManager != null)
            playbackManager.VideoFinished -= OnVideoFinished;
    }
    private void OnVideoFinished(VideoEntry finishedVideo)
    {
        Debug.Log("CHOICE PANEL SHOULD APPEAR NOW");
        ShowChoices();
    }

    private void ShowChoices()
    {
        choicePanel.SetActive(true);

        titleText.text = "Choose Your Next Destination";

        choiceAText.text = "Continue Exploration";
        choiceBText.text = "Explore Another Area";

        choiceButtonA.onClick.RemoveAllListeners();
        choiceButtonB.onClick.RemoveAllListeners();

        choiceButtonA.onClick.AddListener(() =>
        {
            LoadVideo(videoA);
        });

        choiceButtonB.onClick.AddListener(() =>
        {
            LoadVideo(videoB);
        });
    }

    private void LoadVideo(VideoEntry video)
    {
        choicePanel.SetActive(false);

        playbackManager.LoadVideo(video);
    }


}