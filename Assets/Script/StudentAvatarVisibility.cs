
using UnityEngine;

public class StudentAvatarVisibility : MonoBehaviour
{
    private Renderer[] avatarRenderers;
    private bool[] originalRendererStates;

    private void Awake()
    {
        avatarRenderers =
            GetComponentsInChildren<Renderer>(true);

        originalRendererStates =
            new bool[avatarRenderers.Length];

        for (int i = 0; i < avatarRenderers.Length; i++)
        {
            originalRendererStates[i] =
                avatarRenderers[i].enabled;
        }
    }

    public void SetVisible(bool visible)
    {
        if (avatarRenderers == null)
            return;

        for (int i = 0; i < avatarRenderers.Length; i++)
        {
            if (avatarRenderers[i] == null)
                continue;

            avatarRenderers[i].enabled =
                visible && originalRendererStates[i];
        }
    }
}
