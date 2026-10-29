using UnityEngine;

public class VideoLoadingIndicator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loadingCircle;
    [SerializeField] private GameObject readyIndicator;

    [Header("Loading Animation")]
    [SerializeField] private float rotationSpeed = 180f;

    private bool isLoading = false;

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if (isLoading && loadingCircle != null)
        {
            loadingCircle.transform.Rotate(
                0f,
                0f,
                -rotationSpeed * Time.deltaTime
            );
        }
    }

    public void ShowLoading()
    {
        isLoading = true;

        if (loadingCircle != null)
            loadingCircle.SetActive(true);

        if (readyIndicator != null)
            readyIndicator.SetActive(false);

        gameObject.SetActive(true);

        Debug.Log("VideoLoadingIndicator: LOADING");
    }

    public void ShowReady()
    {
        isLoading = false;

        if (loadingCircle != null)
            loadingCircle.SetActive(false);

        if (readyIndicator != null)
            readyIndicator.SetActive(true);

        gameObject.SetActive(true);

        Debug.Log("VideoLoadingIndicator: READY");
    }

    public void Hide()
    {
        isLoading = false;

        if (loadingCircle != null)
            loadingCircle.SetActive(false);

        if (readyIndicator != null)
            readyIndicator.SetActive(false);
    }
}