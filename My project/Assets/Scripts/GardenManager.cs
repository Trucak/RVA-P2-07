using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GardenManager : MonoBehaviour
{
    public static GardenManager Instance { get; private set; }

    public enum TimeOfDay
    {
        Midday,     // Was Summer/PicoDoDia
        Morning,    // Was Spring/Manha
        Evening,    // Was Autumn/FimDeTarde
        Night       // Was Winter/Noite
    }

    [Header("Environment Settings")]
    [FormerlySerializedAs("seasonSkyboxes")]
    [SerializeField] private Material[] timeOfDaySkyboxes;
    [SerializeField] private Light directionalLight;
    
    [FormerlySerializedAs("snowParticleSystem")]
    [SerializeField] private ParticleSystem nightParticleSystem;

    [Header("Sun Settings")]
    [FormerlySerializedAs("seasonSunRotations")]
    [SerializeField] private Vector3[] timeOfDaySunRotations;
    [FormerlySerializedAs("seasonSunColors")]
    [SerializeField] private Color[] timeOfDaySunColors;
    [FormerlySerializedAs("seasonSunIntensities")]
    [SerializeField] private float[] timeOfDaySunIntensities;

    [Header("Ambient Settings")]
    [FormerlySerializedAs("seasonAmbientColors")]
    [SerializeField] private Color[] timeOfDayAmbientColors;
    [FormerlySerializedAs("seasonAmbientIntensities")]
    [SerializeField] private float[] timeOfDayAmbientIntensities;

    [Header("Fog Settings")]
    [FormerlySerializedAs("seasonFogColors")]
    [SerializeField] private Color[] timeOfDayFogColors;

    private Color originalLightColor;
    private float originalLightIntensity;
    private Quaternion originalLightRotation;
    private Color originalAmbientColor;
    private Material originalSkybox;
    private UnityEngine.Rendering.AmbientMode originalAmbientMode;
    private Color originalFogColor;

    [Header("Game State")]
    private int plantCount = 0;
    private TimeOfDay currentTimeOfDay = TimeOfDay.Midday;

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
        
        StopParticles();
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

        StopParticles();

        SetTimeOfDay(TimeOfDay.Midday);
    }
    
    private void StopParticles()
    {
        if (nightParticleSystem != null)
        {
            // Forcefully clear all existing particles
            nightParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void RegisterPlant()
    {
        plantCount++;
        // Optional: Affect environment based on plants if needed
    }

    public void SetTimeOfDay(TimeOfDay newTime)
    {
        currentTimeOfDay = newTime;
        int index = (int)currentTimeOfDay;
        Debug.Log("Setting time of day to: " + currentTimeOfDay);
        
        // Reset effects first
        StopParticles();
        
        UpdateSkybox(index);

        if (directionalLight != null)
        {
            if (timeOfDaySunColors != null && index < timeOfDaySunColors.Length) 
                directionalLight.color = timeOfDaySunColors[index];
            
            if (timeOfDaySunIntensities != null && index < timeOfDaySunIntensities.Length) 
                directionalLight.intensity = timeOfDaySunIntensities[index];
            
            if (timeOfDaySunRotations != null && index < timeOfDaySunRotations.Length) 
                directionalLight.transform.rotation = Quaternion.Euler(timeOfDaySunRotations[index]);
        }

        if (index == 0) // Midday (Default)
        {
             RenderSettings.ambientMode = originalAmbientMode;
             RenderSettings.ambientLight = originalAmbientColor;
        }
        else
        {
             RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
             if (timeOfDayAmbientColors != null && index < timeOfDayAmbientColors.Length) 
                RenderSettings.ambientLight = timeOfDayAmbientColors[index];
        }

        if (timeOfDayAmbientIntensities != null && index < timeOfDayAmbientIntensities.Length)
        {
            RenderSettings.ambientIntensity = timeOfDayAmbientIntensities[index];
        }

        if (timeOfDayFogColors != null && index < timeOfDayFogColors.Length)
        {
            RenderSettings.fogColor = timeOfDayFogColors[index];
        }

        if (currentTimeOfDay == TimeOfDay.Night && nightParticleSystem != null)
        {
            nightParticleSystem.Play();
        }

        DynamicGI.UpdateEnvironment();
    }

    private void UpdateSkybox(int index)
    {
        if (timeOfDaySkyboxes != null && index < timeOfDaySkyboxes.Length && timeOfDaySkyboxes[index] != null)
        {
            RenderSettings.skybox = timeOfDaySkyboxes[index];
        }
        else if (index == 0 && originalSkybox != null)
        {
             RenderSettings.skybox = originalSkybox;
        }
        else if (index == 3) // Night Fallback (if no skybox assigned)
        {
             RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
             RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.2f); // Darker Blue for Night
        }
    }

    public void RestartGarden()
    {
        plantCount = 0;
        SetTimeOfDay(TimeOfDay.Midday);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
