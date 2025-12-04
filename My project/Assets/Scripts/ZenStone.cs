using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ZenStone : MonoBehaviour
{
    [Header("Zen Mechanics")]
    [SerializeField] private float stabilityThreshold = 5.0f;
    [SerializeField] private float grabGracePeriod = 0.5f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private float grabStartTime;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.centerOfMass = new Vector3(0, -0.02f, 0);
        }
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        grabStartTime = Time.time;
        lastPosition = transform.position;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false; 
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
    
        if (rb != null)
        {
            rb.isKinematic = false; 
            rb.useGravity = true;  
            
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;

            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.05f;
        }
    }

    private bool isGrounded = false;

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        if (rb != null)
        {
            rb.linearDamping = 5f;
            rb.angularDamping = 5f; 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        if (rb != null)
        {
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.05f;
        }
    }

    private void FixedUpdate()
    {
        if (grabInteractable.isSelected)
        {
            CheckStability();
        }
        else
        {
            if (rb != null && !rb.isKinematic && rb.useGravity)
            {
                if (rb.linearVelocity.y < 0.1f && !isGrounded)
                {
                    rb.AddForce(Physics.gravity * 2.0f, ForceMode.Acceleration);
                }

                if (rb.linearVelocity.magnitude < 0.05f && rb.angularVelocity.magnitude < 0.05f)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.Sleep();
                }
            }
            
            lastPosition = transform.position;
        }
    }

    private void CheckStability()
    {
        if (Time.time - grabStartTime < grabGracePeriod)
        {
            lastPosition = transform.position;
            return;
        }

        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (speed > stabilityThreshold)
        {
            Debug.Log($"Speed {speed} > Threshold {stabilityThreshold}. Dropping!");
            ForceDrop();
        }

        lastPosition = transform.position;
    }

    private void ForceDrop()
    {
        if (grabInteractable.interactorsSelecting.Count > 0)
        {
            var interactors = new System.Collections.Generic.List<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>(grabInteractable.interactorsSelecting);
            var manager = grabInteractable.interactionManager;
            
            if (manager != null)
            {
                foreach (var interactor in interactors)
                {
                    manager.SelectExit(interactor, grabInteractable);
                }
                Debug.Log("Zen Stone dropped due to fast movement!");
                
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.linearVelocity = Vector3.zero; 
                    rb.linearDamping = 0.05f;
                    rb.angularDamping = 0.05f;
                }
            }
        }
    }
}
