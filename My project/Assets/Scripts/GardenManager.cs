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

    [Header("Season Operations")]
    [SerializeField] private Color summerLightColor = new Color(1f, 0.95f, 0.8f); // Warm Sunlight
    [SerializeField] private Color springAmbientColor = new Color(0.6f, 0.5f, 0.4f); // Brownish
    [SerializeField] private Color autumnAmbientColor = new Color(0.4f, 0.2f, 0.1f); // Dark Brown
    [SerializeField] private float autumnLightIntensityMultiplier = 0.5f;

    private Color originalLightColor;
    private float originalLightIntensity;
    private Color originalAmbientColor;
    private Material originalSkybox;
    private UnityEngine.Rendering.AmbientMode originalAmbientMode;

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
        }
        originalAmbientColor = RenderSettings.ambientLight;
        originalSkybox = RenderSettings.skybox;
        originalAmbientMode = RenderSettings.ambientMode;

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
        Debug.Log("Setting season to: " + currentSeason);
        
        // Reset effects first
        StopSnow();
        
        // Restore defaults first (baseline)
        if (directionalLight != null)
        {
            directionalLight.intensity = originalLightIntensity;
            directionalLight.color = originalLightColor; // Restore original color
        }
        RenderSettings.ambientLight = originalAmbientColor;
        RenderSettings.ambientMode = originalAmbientMode;
        
        switch (currentSeason)
        {
            case Season.Summer:
                // Summer acts as "Default/Reset" - clear all season overrides
                // No explicit calls needed as defaults are restored above
                StopSnow(); 
                UpdateSkybox(0); 
                break;
                
            case Season.Spring:
                // Spring: Summer light + acastanhado (brownish) ambient
                // If you want Spring to use "Summer Color" vs "Original", use originalLightColor or summerLightColor field.
                // Assuming user wants "Original" as base for everything now.
                if (directionalLight != null) directionalLight.color = summerLightColor; // Keep using this warm color for Spring? Or Original?
                // User only complained about Summer. I'll keep Spring using the warm helper variable if they want, or I should use original?
                // Let's use summerLightColor for Spring/Autumn overrides if it enhances them, BUT logic says Spring is based on "Summer".
                // I will keep Spring as is for now to avoid breaking it.
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                RenderSettings.ambientLight = springAmbientColor;
                StopSnow();
                UpdateSkybox(1);
                break;
                
            case Season.Autumn:
                // Autumn: Much more brown and dark
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                RenderSettings.ambientLight = autumnAmbientColor;
                if (directionalLight != null)
                {
                    directionalLight.color = new Color(0.8f, 0.6f, 0.4f); // Warm/dim light
                    directionalLight.intensity = originalLightIntensity * autumnLightIntensityMultiplier;
                }
                StopSnow();
                UpdateSkybox(2);
                break;
                
            case Season.Winter:
                // Winter: Snow falling + Darker Blue Light (Real Winter)
                 if (directionalLight != null) 
                 {
                     directionalLight.color = new Color(0.6f, 0.75f, 1.0f); // Stronger Blue
                     directionalLight.intensity = originalLightIntensity * 0.6f; // Darker
                 }
                if (snowParticleSystem != null) snowParticleSystem.Play();
                UpdateSkybox(3);
                break;
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
