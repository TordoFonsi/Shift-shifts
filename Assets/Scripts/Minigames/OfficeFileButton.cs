using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfficeFileButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text stamp;
    [SerializeField] private Image background;

    private Color stampColor;

    public void Setup(Color color)
    {
        stampColor = color;
        stamp.color = stampColor;
        SetSelected(false, false);
    }

    public void SetSelected(bool selected, bool active)
    {
        if (!selected)
            background.color = new Color(0.16f, 0.19f, 0.24f);
        else if (active)
            background.color = new Color(0.25f, 0.55f, 0.85f);
        else
            background.color = new Color(0.20f, 0.35f, 0.50f);
    }
}
