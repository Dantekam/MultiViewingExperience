
using UnityEngine;

public class StudentAvatarVisibility : MonoBehaviour
{
    private Renderer[] avatarRenderers;
    private bool hidden;

    private void Awake()
    {
        avatarRenderers =
            GetComponentsInChildren<Renderer>(true);
    }

    public void SetVisible(bool visible)
    {
        hidden = !visible;
        ApplyVisibility();
    }

    private void LateUpdate()
    {
        if (hidden)
        {
            ApplyVisibility();
        }
    }

    private void ApplyVisibility()
    {
        if (avatarRenderers == null)
            return;

        foreach (Renderer avatarRenderer in avatarRenderers)
        {
            if (avatarRenderer == null)
                continue;

            avatarRenderer.forceRenderingOff = hidden;
        }
    }
}
