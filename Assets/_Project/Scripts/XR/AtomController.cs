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
        [SerializeField] private TMPro.TextMeshPro atomLabel;

        private XRGrabInteractable grabInteractable;

        public AtomType AtomType => atomType;
        public XRGrabInteractable GrabInteractable => grabInteractable;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();

            if (atomLabel != null)
            {
                atomLabel.text = GetShortAtomLabel(atomType);
            }
        }

        private string GetShortAtomLabel(AtomType type)
        {
            return type switch
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