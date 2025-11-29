using UnityEngine;
using UnityEngine.SceneManagement;

public class GardenManager : MonoBehaviour
{
    public static GardenManager Instance { get; private set; }

    [Header("Environment Settings")]
    [SerializeField] private Material[] seasonSkyboxes;
    [SerializeField] private Light directionalLight;
    [SerializeField] private float lightIntensityStep = 0.1f;
    [SerializeField] private float maxLightIntensity = 2.0f;

    [Header("Game State")]
    private int plantCount = 0;
    private int currentSeasonIndex = 0;
    private string[] seasons = { "Spring", "Summer", "Autumn", "Winter" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateEnvironment();
        UpdateSeasonVisuals();
    }

    public void RegisterPlant()
    {
        plantCount++;
        UpdateEnvironment();
    }

    private void UpdateEnvironment()
    {
        // Make the sky/light clearer as plants grow
        if (directionalLight != null)
        {
            float targetIntensity = 1.0f + (plantCount * lightIntensityStep);
            directionalLight.intensity = Mathf.Clamp(targetIntensity, 0.5f, maxLightIntensity);
        }
    }

    public void ChangeSeason()
    {
        currentSeasonIndex = (currentSeasonIndex + 1) % seasons.Length;
        string season = seasons[currentSeasonIndex];
        Debug.Log("Changed season to: " + season);
        
        UpdateSeasonVisuals();
    }

    private void UpdateSeasonVisuals()
    {
        if (seasonSkyboxes != null && seasonSkyboxes.Length > 0)
        {
            int index = currentSeasonIndex % seasonSkyboxes.Length;
            if (seasonSkyboxes[index] != null)
            {
                RenderSettings.skybox = seasonSkyboxes[index];
                DynamicGI.UpdateEnvironment();
            }
        }
    }

    public void RestartGarden()
    {
        plantCount = 0;
        currentSeasonIndex = 0;
        // Reload the current scene to reset objects
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
