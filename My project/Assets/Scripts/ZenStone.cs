using UnityEngine;


public class ZenStone : MonoBehaviour
{
    [Header("Zen Mechanics")]
    [SerializeField] private float stabilityThreshold = 2.0f; // Velocity threshold for dropping
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Vector3 lastPosition;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (grabInteractable.isSelected)
        {
            CheckStability();
        }
    }

    private void CheckStability()
    {
        // Calculate velocity manually or use Rigidbody if non-kinematic
        // When held, it might be kinematic.
        
        float speed = 0f;
        if (rb != null && !rb.isKinematic)
        {
            speed = rb.linearVelocity.magnitude;
        }
        else
        {
            // Estimate speed based on position change
            speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        }

        if (speed > stabilityThreshold)
        {
            ForceDrop();
        }

        lastPosition = transform.position;
    }

    private void ForceDrop()
    {
        // Force the interactor to drop the object
        if (grabInteractable.interactorsSelecting.Count > 0)
        {
            var interactor = grabInteractable.interactorsSelecting[0];
            var manager = grabInteractable.interactionManager;
            
            if (manager != null)
            {
                manager.SelectExit(interactor, grabInteractable);
                Debug.Log("Zen Stone dropped due to fast movement!");
            }
        }
    }
}
