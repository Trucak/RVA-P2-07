using UnityEngine;


public class Seed : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private string groundTag = "Ground";

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if colliding with ground
        if (collision.gameObject.CompareTag(groundTag))
        {
            // Check if the seed is currently being held
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                // Optional: Don't plant if still holding? 
                // Or maybe "placing" implies holding it against the ground.
                // Let's assume we plant when it touches the ground, even if held, 
                // or maybe better: only if released. 
                // The prompt says "Pegar -> colocar -> cresce". 
                // "Colocar" usually means putting it down.
                // Let's allow planting while holding for smoother interaction, 
                // or wait for release. Let's wait for release to avoid accidents.
                return; 
            }

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
