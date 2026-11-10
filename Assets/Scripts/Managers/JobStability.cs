using UnityEngine;
using UnityEngine.UI;

public class JobStability : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image officeFill;
    [SerializeField] private Image taxiFill;

    [Header("Stability Settings")]
    [SerializeField] private float startingStability = 100f;
    [SerializeField] private float recoveryAmount = 25f;
    [SerializeField] private float penaltyAmount = 8f;

    [Header("Drain Settings")]
    [SerializeField] private float initialDrainRate = 1.5f;
    [SerializeField] private float difficultyInterval = 15f;
    [SerializeField] private float drainMultiplier = 1.25f;

    private float officeStability;
    private float taxiStability;
    private float elapsedTime;

    private void Update()
    {
        if (!gameManager.IsPlaying)
            return;

        elapsedTime += Time.deltaTime;

        int difficultyStage =
            Mathf.FloorToInt(elapsedTime / difficultyInterval);

        float currentDrainRate =
            initialDrainRate *
            Mathf.Pow(drainMultiplier, difficultyStage);

        officeStability -= currentDrainRate * Time.deltaTime;
        taxiStability -= currentDrainRate * Time.deltaTime;

        officeStability =
            Mathf.Clamp(officeStability, 0f, startingStability);

        taxiStability =
            Mathf.Clamp(taxiStability, 0f, startingStability);

        UpdateBars();

        if (officeStability <= 0f || taxiStability <= 0f)
        {
            gameManager.LoseGame();
        }
    }

    public void ResetStability()
    {
        officeStability = startingStability;
        taxiStability = startingStability;
        elapsedTime = 0f;

        UpdateBars();
    }

    public void CompleteOfficeTask()
    {
        officeStability =
            Mathf.Min(
                startingStability,
                officeStability + recoveryAmount
            );

        UpdateBars();
    }

    public void CompleteTaxiTask()
    {
        taxiStability =
            Mathf.Min(
                startingStability,
                taxiStability + recoveryAmount
            );

        UpdateBars();
    }

    public void PenalizeOffice()
    {
        officeStability =
            Mathf.Max(0f, officeStability - penaltyAmount);

        UpdateBars();

        if (officeStability <= 0f)
            gameManager.LoseGame();
    }

    public void PenalizeTaxi()
    {
        taxiStability =
            Mathf.Max(0f, taxiStability - penaltyAmount);

        UpdateBars();

        if (taxiStability <= 0f)
            gameManager.LoseGame();
    }

    private void UpdateBars()
    {
        officeFill.fillAmount =
            officeStability / startingStability;

        taxiFill.fillAmount =
            taxiStability / startingStability;
    }
}
