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
            ClearVisual();

            if (moleculeDefinition == null || moleculeDefinition.MoleculePrefab == null)
            {
                return;
            }

            if (visualAnchor == null)
            {
                Debug.LogWarning("MoleculeVisualPresenter has no visual anchor assigned.", this);
                return;
            }

            currentVisualInstance = Instantiate(
                moleculeDefinition.MoleculePrefab,
                visualAnchor.position,
                visualAnchor.rotation,
                visualAnchor);
        }

        public void ClearVisual()
        {
            if (currentVisualInstance != null)
            {
                Destroy(currentVisualInstance);
                currentVisualInstance = null;
            }
        }
    }
}