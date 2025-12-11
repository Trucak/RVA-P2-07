using UnityEngine;
using UnityEngine.InputSystem;

public class HandMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private InputActionProperty toggleMenuAction;

    private void Start()
    {
        // Ensure menu is hidden at start if reference is assigned
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (toggleMenuAction.action != null)
            toggleMenuAction.action.Enable();
    }

    private void OnDisable()
    {
        if (toggleMenuAction.action != null)
            toggleMenuAction.action.Disable();
    }

    private void Update()
    {
        if (toggleMenuAction.action != null && toggleMenuAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }
    }

    public void OnSummerPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.SetSeason(GardenManager.Season.Summer);
        }
    }

    public void OnSpringPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.SetSeason(GardenManager.Season.Spring);
        }
    }

    public void OnAutumnPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.SetSeason(GardenManager.Season.Autumn);
        }
    }

    public void OnWinterPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.SetSeason(GardenManager.Season.Winter);
        }
    }

    public void OnRestartGardenPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.RestartGarden();
        }
    }
}
