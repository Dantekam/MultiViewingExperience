using UnityEngine;
using UnityEngine.UI;

public class VideoSelectionVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Graphic targetGraphic;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;

    [SerializeField] private Color selectedColor =
        new Color(0.55f, 0.75f, 1f, 1f);

    public void SetSelected(bool selected)
    {
        if (targetGraphic == null)
        {
            Debug.LogWarning(
                $"VideoSelectionVisual: Target Graphic missing on {gameObject.name}");

            return;
        }

        targetGraphic.color =
            selected
                ? selectedColor
                : normalColor;
    }
}