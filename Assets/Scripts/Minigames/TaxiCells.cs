using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaxiCell : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text symbol;

    private TaxiMinigame minigame;
    private int cellIndex;

    public void Setup(TaxiMinigame manager, int index)
    {
        minigame = manager;
        cellIndex = index;

        Button button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(
            () => minigame.SelectCell(cellIndex)
        );
    }

    public void SetCell(Color color, string text)
    {
        background.color = color;
        symbol.text = text;
    }
}
