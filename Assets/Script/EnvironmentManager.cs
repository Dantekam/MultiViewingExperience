using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Environment Objects To Hide")]
    [SerializeField]
    private List<GameObject> objectsToHide = new List<GameObject>();

    [Header("Floor")]
    [Tooltip("Drag the floor parent/root GameObject here.")]
    [SerializeField]
    private GameObject floorRoot;

    private Renderer[] floorRenderers;

    private void Awake()
    {
        if (floorRoot != null)
        {
            // Finds every renderer on the floor and its children.
            floorRenderers =
                floorRoot.GetComponentsInChildren<Renderer>(true);
        }
        else
        {
            floorRenderers = new Renderer[0];

            Debug.LogWarning(
                "EnvironmentManager: Floor Root is not assigned.");
        }
    }

    public void StartExperience()
    {
        Debug.Log("EnvironmentManager: Starting experience.");

        // Hide environment objects.
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // Hide the floor visually WITHOUT disabling its collider.
        foreach (Renderer floorRenderer in floorRenderers)
        {
            if (floorRenderer != null)
            {
                floorRenderer.enabled = false;
            }
        }

        Debug.Log(
            $"EnvironmentManager: Disabled {floorRenderers.Length} floor renderer(s).");
    }

    public void ShowEnvironment()
    {
        // Bring environment back if needed later.
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // Make the floor visible again.
        foreach (Renderer floorRenderer in floorRenderers)
        {
            if (floorRenderer != null)
            {
                floorRenderer.enabled = true;
            }
        }

        Debug.Log("EnvironmentManager: Environment restored.");
    }
}