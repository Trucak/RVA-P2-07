using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private string groundTag = "Terrain";

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                return; 
            }

            SeedSpawner spawner = collision.gameObject.GetComponent<SeedSpawner>();
            if (spawner != null) return; 

            PlantSeed(collision.contacts[0].point);
        }
    }

    private void PlantSeed(Vector3 position)
    {
        if (plantPrefab != null)
        {
            Instantiate(plantPrefab, position, Quaternion.identity);
            
            if (GardenManager.Instance != null)
            {
                GardenManager.Instance.RegisterPlant();
            }

            Destroy(gameObject);
        }
    }
}
