    using UnityEngine;

public class JobSwitcher : MonoBehaviour
{
    private enum Job
    {
        Office,
        Taxi
    }

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject officePanel;
    [SerializeField] private GameObject taxiPanel;

    [Header("Settings")]
    [SerializeField] private bool startWithOffice = true;

    private Job currentJob;

    public bool IsOfficeActive =>
        currentJob == Job.Office;

    public bool IsTaxiActive =>
        currentJob == Job.Taxi;

    private void Update()
    {
        if (!gameManager.IsPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift))
        {
            SwitchJob();
        }
    }

    public void ResetJobs()
    {
        currentJob =
            startWithOffice ? Job.Office : Job.Taxi;

        UpdatePanels();
    }

    private void SwitchJob()
    {
        currentJob =
            currentJob == Job.Office
                ? Job.Taxi
                : Job.Office;

        UpdatePanels();
    }

    private void UpdatePanels()
    {
        if (officePanel != null)
            officePanel.SetActive(IsOfficeActive);

        if (taxiPanel != null)
            taxiPanel.SetActive(IsTaxiActive);
    }
}
