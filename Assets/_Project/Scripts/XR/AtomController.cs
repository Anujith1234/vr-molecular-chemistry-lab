using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Data;

namespace VRMolecularLab.XR
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class AtomController : MonoBehaviour
    {
        [Header("Atom Identity")]
        [SerializeField] private AtomType atomType;

        [Header("Optional References")]
        [SerializeField] private Renderer atomRenderer;
        [SerializeField] private TextMeshPro atomLabel;

        private XRGrabInteractable grabInteractable;

        public AtomType AtomType => atomType;
        public XRGrabInteractable GrabInteractable => grabInteractable;
        public Renderer AtomRenderer => atomRenderer;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            RefreshLabel();
        }

        private void RefreshLabel()
        {
            if (atomLabel == null)
            {
                return;
            }

            atomLabel.text = atomType switch
            {
                AtomType.Hydrogen => "H",
                AtomType.Oxygen => "O",
                AtomType.Carbon => "C",
                AtomType.Nitrogen => "N",
                _ => "?"
            };
        }
    }
}