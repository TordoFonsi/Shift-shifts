using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OfficeMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private JobStability jobStability;
    [SerializeField] private JobSwitcher jobSwitcher;
    [SerializeField] private Transform columnA;
    [SerializeField] private Transform columnB;
    [SerializeField] private OfficeFileButton filePrefab;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Stamp Colors")]
    [SerializeField]
    private Color[] stampColors =
    {
        new Color(0.90f, 0.20f, 0.20f),
        new Color(0.20f, 0.55f, 0.95f),
        new Color(0.20f, 0.75f, 0.35f),
        new Color(0.95f, 0.75f, 0.15f),
        new Color(0.70f, 0.35f, 0.90f),
        new Color(0.95f, 0.40f, 0.15f),
        new Color(0.15f, 0.80f, 0.80f),
        new Color(0.95f, 0.40f, 0.65f),
        new Color(0.55f, 0.70f, 0.25f)
    };

    private readonly List<Color> colorsA = new List<Color>();
    private readonly List<Color> colorsB = new List<Color>();

    private readonly List<OfficeFileButton> buttonsA =
        new List<OfficeFileButton>();

    private readonly List<OfficeFileButton> buttonsB =
        new List<OfficeFileButton>();

    private int selectedA;
    private int selectedB;
    private int activeColumn;

    private void Update()
    {
        if (!gameManager.IsPlaying) return;
        if (!jobSwitcher.IsOfficeActive) return;

        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.LeftArrow))
        {
            activeColumn = 0;
            UpdateSelectionVisuals();
        }

        if (Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.RightArrow))
        {
            activeColumn = 1;
            UpdateSelectionVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }

    public void ResetMinigame()
    {
        GenerateFiles();
    }

    private void MoveSelection(int direction)
    {
        if (activeColumn == 0)
        {
            selectedA += direction;

            if (selectedA < 0)
                selectedA = buttonsA.Count - 1;

            if (selectedA >= buttonsA.Count)
                selectedA = 0;
        }
        else
        {
            selectedB += direction;

            if (selectedB < 0)
                selectedB = buttonsB.Count - 1;

            if (selectedB >= buttonsB.Count)
                selectedB = 0;
        }

        UpdateSelectionVisuals();
    }

    private void ConfirmSelection()
    {
        if (buttonsA.Count == 0 || buttonsB.Count == 0)
            return;

        Color selectedColorA = colorsA[selectedA];
        Color selectedColorB = colorsB[selectedB];

        if (selectedColorA == selectedColorB)
        {
            jobStability.CompleteOfficeTask();
            GenerateFiles();
            feedbackText.text = "DUPLICATE FOUND! +25 OFFICE";
        }
        else
        {
            feedbackText.text = "WRONG FILES! -8 OFFICE";
            jobStability.PenalizeOffice();
        }
    }

    private void GenerateFiles()
    {
        ClearFiles();

        if (stampColors == null || stampColors.Length < 9)
        {
            Debug.LogError(
                "OfficeMinigame needs at least 9 stamp colors."
            );
            return;
        }

        List<Color> availableColors =
            new List<Color>(stampColors);

        Shuffle(availableColors);

        Color duplicateColor = availableColors[0];

        colorsA.Add(duplicateColor);
        colorsB.Add(duplicateColor);

        for (int i = 1; i <= 4; i++)
            colorsA.Add(availableColors[i]);

        for (int i = 5; i <= 8; i++)
            colorsB.Add(availableColors[i]);

        Shuffle(colorsA);
        Shuffle(colorsB);

        selectedA = 0;
        selectedB = 0;
        activeColumn = 0;

        for (int i = 0; i < colorsA.Count; i++)
        {
            OfficeFileButton button =
                Instantiate(filePrefab, columnA);

            button.Setup(colorsA[i]);
            buttonsA.Add(button);
        }

        for (int i = 0; i < colorsB.Count; i++)
        {
            OfficeFileButton button =
                Instantiate(filePrefab, columnB);

            button.Setup(colorsB[i]);
            buttonsB.Add(button);
        }

        feedbackText.text =
            "FIND THE TWO MATCHING STAMPS!";

        UpdateSelectionVisuals();
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < buttonsA.Count; i++)
        {
            buttonsA[i].SetSelected(
                selectedA == i,
                activeColumn == 0
            );
        }

        for (int i = 0; i < buttonsB.Count; i++)
        {
            buttonsB[i].SetSelected(
                selectedB == i,
                activeColumn == 1
            );
        }
    }

    private void ClearFiles()
    {
        colorsA.Clear();
        colorsB.Clear();
        buttonsA.Clear();
        buttonsB.Clear();

        for (int i = columnA.childCount - 1; i >= 0; i--)
            Destroy(columnA.GetChild(i).gameObject);

        for (int i = columnB.childCount - 1; i >= 0; i--)
            Destroy(columnB.GetChild(i).gameObject);
    }

    private void Shuffle(List<Color> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }
}
