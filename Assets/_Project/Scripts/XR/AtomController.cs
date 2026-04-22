using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Data;

namespace VRMolecularLab.XR
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class AtomController : MonoBehaviour
    {
        private static readonly Color CarbonPreviewEmission = new(0.35f, 0.35f, 0.35f, 1f);
        private const float DefaultPreviewEmissionMultiplier = 3f;

        [Header("Atom Identity")]
        [SerializeField] private AtomType atomType;

        [Header("Optional References")]
        [SerializeField] private Renderer atomRenderer;
        [SerializeField] private TextMeshPro atomLabel;

        private XRGrabInteractable grabInteractable;
        private Material runtimeMaterial;
        private Color originalColor;
        private Color originalEmission;

        private bool hasCachedVisualState;
        private bool hasEmission;
        private bool isBondingPreviewActive;

        public AtomType AtomType => atomType;
        public XRGrabInteractable GrabInteractable => grabInteractable;
        public Renderer AtomRenderer => atomRenderer;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            RefreshLabel();
            CacheVisualState();
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

        private void CacheVisualState()
        {
            if (atomRenderer == null)
            {
                return;
            }

            runtimeMaterial = atomRenderer.material;

            if (runtimeMaterial.HasProperty("_Color"))
            {
                originalColor = runtimeMaterial.color;
                hasCachedVisualState = true;
            }

            if (runtimeMaterial.HasProperty("_EmissionColor"))
            {
                originalEmission = runtimeMaterial.GetColor("_EmissionColor");
                hasEmission = true;
            }
        }

        public void SetBondingPreview(bool isActive)
        {
            if (!hasCachedVisualState || runtimeMaterial == null)
            {
                return;
            }

            if (isBondingPreviewActive == isActive)
            {
                return;
            }

            isBondingPreviewActive = isActive;

            runtimeMaterial.color = originalColor;

            if (!hasEmission)
            {
                return;
            }

            runtimeMaterial.EnableKeyword("_EMISSION");
            runtimeMaterial.SetColor("_EmissionColor", isActive ? GetPreviewEmissionColor() : originalEmission);
        }

        private Color GetPreviewEmissionColor()
        {
            return atomType == AtomType.Carbon
                ? CarbonPreviewEmission
                : originalColor * DefaultPreviewEmissionMultiplier;
        }
    }
}