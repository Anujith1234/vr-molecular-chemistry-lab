using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.XR;

namespace VRMolecularLab.Molecules
{
    [RequireComponent(typeof(AtomController))]
    public class AtomResettable : MonoBehaviour
    {
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private Rigidbody rb;
        private XRGrabInteractable grabInteractable;

        private bool isConsumed;

        private void Awake()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            rb = GetComponent<Rigidbody>();
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        public void Consume()
        {
            if (isConsumed)
                return;

            isConsumed = true;

            // Disable XR interaction first to prevent grab conflicts
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }

            // Stop physics completely
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            gameObject.SetActive(false);
        }

        public void RestoreToInitialState()
        {
            if (!isConsumed)
                return;

            isConsumed = false;

            gameObject.SetActive(true);

            transform.SetPositionAndRotation(initialPosition, initialRotation);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (grabInteractable != null)
            {
                grabInteractable.enabled = true;
            }
        }
    }
}