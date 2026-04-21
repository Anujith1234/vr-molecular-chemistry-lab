using UnityEngine;

namespace VRMolecularLab.Molecules
{
    public class MoleculeVisualPresenter : MonoBehaviour
    {
        [Header("Spawn Setup")]
        [SerializeField] private Transform visualAnchor;

        private GameObject currentVisualInstance;

        public void ShowMolecule(MoleculeDefinition moleculeDefinition)
        {
            if (visualAnchor == null)
            {
                Debug.LogWarning("MoleculeVisualPresenter is missing its visual anchor.", this);
                return;
            }

            if (moleculeDefinition == null || moleculeDefinition.MoleculePrefab == null)
            {
                ClearVisual();
                return;
            }

            ClearVisual();

            currentVisualInstance = Instantiate(
                moleculeDefinition.MoleculePrefab,
                visualAnchor.position,
                visualAnchor.rotation,
                visualAnchor
            );
        }

        public void ClearVisual()
        {
            if (currentVisualInstance == null)
                return;

            Destroy(currentVisualInstance);
            currentVisualInstance = null;
        }
    }
}