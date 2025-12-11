using UnityEngine;
using UnityEngine.SceneManagement;

public class GardenManager : MonoBehaviour
{
    public static GardenManager Instance { get; private set; }

    public enum Season
    {
        Summer,
        Spring,
        Autumn,
        Winter
    }

    [Header("Environment Settings")]
    [SerializeField] private Material[] seasonSkyboxes;
    [SerializeField] private Light directionalLight;
    [SerializeField] private ParticleSystem snowParticleSystem;

    [Header("Sun Settings")]
    [SerializeField] private Vector3[] seasonSunRotations;
    [SerializeField] private Color[] seasonSunColors;
    [SerializeField] private float[] seasonSunIntensities;

    [Header("Ambient Settings")]
    [SerializeField] private Color[] seasonAmbientColors;
    [SerializeField] private float[] seasonAmbientIntensities;

    [Header("Fog Settings")]
    [SerializeField] private Color[] seasonFogColors;

    private Color originalLightColor;
    private float originalLightIntensity;
    private Quaternion originalLightRotation;
    private Color originalAmbientColor;
    private Material originalSkybox;
    private UnityEngine.Rendering.AmbientMode originalAmbientMode;
    private Color originalFogColor;

    [Header("Game State")]
    private int plantCount = 0;
    private Season currentSeason = Season.Summer;

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
        
        // Ensure snow is dead on arrival
        StopSnow();
    }

    private void Start()
    {
        // Store original settings
        if (directionalLight != null)
        {
            originalLightColor = directionalLight.color;
            originalLightIntensity = directionalLight.intensity;
            originalLightRotation = directionalLight.transform.rotation;
        }
        originalAmbientColor = RenderSettings.ambientLight;
        originalSkybox = RenderSettings.skybox;
        originalAmbientMode = RenderSettings.ambientMode;
        originalFogColor = RenderSettings.fogColor;

        StopSnow();

        SetSeason(Season.Summer);
    }
    
    private void StopSnow()
    {
        if (snowParticleSystem != null)
        {
            // Forcefully clear all existing particles
            snowParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void RegisterPlant()
    {
        plantCount++;
        // Optional: Affect environment based on plants if needed
    }

    public void SetSeason(Season newSeason)
    {
        currentSeason = newSeason;
        int index = (int)currentSeason;
        Debug.Log("Setting season to: " + currentSeason);
        
        // Reset effects first
        StopSnow();
        
        UpdateSkybox(index);

        if (directionalLight != null)
        {
            // Apply overrides for all seasons (including Summer)
            if (seasonSunColors != null && index < seasonSunColors.Length) 
                directionalLight.color = seasonSunColors[index];
            
            if (seasonSunIntensities != null && index < seasonSunIntensities.Length) 
                directionalLight.intensity = seasonSunIntensities[index];
            
            if (seasonSunRotations != null && index < seasonSunRotations.Length) 
                directionalLight.transform.rotation = Quaternion.Euler(seasonSunRotations[index]);
        }

        if (index == 0) // Summer (Default)
        {
             RenderSettings.ambientMode = originalAmbientMode;
             RenderSettings.ambientLight = originalAmbientColor;
        }
        else
        {
             RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
             if (seasonAmbientColors != null && index < seasonAmbientColors.Length) 
                RenderSettings.ambientLight = seasonAmbientColors[index];
        }

        if (seasonAmbientIntensities != null && index < seasonAmbientIntensities.Length)
        {
            RenderSettings.ambientIntensity = seasonAmbientIntensities[index];
        }

        if (seasonFogColors != null && index < seasonFogColors.Length)
        {
            RenderSettings.fogColor = seasonFogColors[index];
        }

        if (currentSeason == Season.Winter && snowParticleSystem != null)
        {
            snowParticleSystem.Play();
        }

        DynamicGI.UpdateEnvironment();
    }

    private void UpdateSkybox(int index)
    {
        if (seasonSkyboxes != null && index < seasonSkyboxes.Length && seasonSkyboxes[index] != null)
        {
            RenderSettings.skybox = seasonSkyboxes[index];
        }
        else if (index == 0 && originalSkybox != null)
        {
             RenderSettings.skybox = originalSkybox;
        }
        else if (index == 3) // Winter Fallback (if no skybox assigned)
        {
             RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
             RenderSettings.ambientLight = new Color(0.2f, 0.25f, 0.45f); // Dark Blue Night/Winter
        }
    }

    public void RestartGarden()
    {
        plantCount = 0;
        SetSeason(Season.Summer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
