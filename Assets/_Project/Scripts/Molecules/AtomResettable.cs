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

        private Rigidbody cachedRigidbody;
        private XRGrabInteractable cachedGrabInteractable;

        private void Start()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            cachedRigidbody = GetComponent<Rigidbody>();
            cachedGrabInteractable = GetComponent<XRGrabInteractable>();
        }

        public void Consume()
        {
            if (cachedGrabInteractable != null)
            {
                cachedGrabInteractable.enabled = false;
            }

            if (cachedRigidbody != null)
            {
                cachedRigidbody.isKinematic = false;
                cachedRigidbody.linearVelocity = Vector3.zero;
                cachedRigidbody.angularVelocity = Vector3.zero;
                cachedRigidbody.isKinematic = true;
            }

            gameObject.SetActive(false);
        }

        public void RestoreToInitialState()
        {
            gameObject.SetActive(true);

            transform.position = initialPosition;
            transform.rotation = initialRotation;

            if (cachedRigidbody != null)
            {
                cachedRigidbody.isKinematic = false;
                cachedRigidbody.linearVelocity = Vector3.zero;
                cachedRigidbody.angularVelocity = Vector3.zero;
            }

            if (cachedGrabInteractable != null)
            {
                cachedGrabInteractable.enabled = true;
            }
        }
    }
}