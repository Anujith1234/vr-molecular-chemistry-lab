using TMPro;
using UnityEngine;

namespace VRMolecularLab.Molecules
{
    public class MoleculeResultDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject resultRoot;
        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private TextMeshPro formulaText;
        [SerializeField] private TextMeshPro bondText;

        [Header("Fallback Text")]
        [SerializeField] private string emptyNameText = "No Molecule";
        [SerializeField] private string emptyFormulaText = "--";
        [SerializeField] private string emptyBondText = "--";

        private void Start()
        {
            ClearDisplay();
        }

        public void ShowMolecule(MoleculeDefinition moleculeDefinition)
        {
            if (moleculeDefinition == null)
            {
                ClearDisplay();
                return;
            }

            if (resultRoot != null)
            {
                resultRoot.SetActive(true);
            }

            if (nameText != null)
            {
                nameText.text = moleculeDefinition.MoleculeName;
            }

            if (formulaText != null)
            {
                formulaText.text = moleculeDefinition.Formula;
            }

            if (bondText != null)
            {
                bondText.text = moleculeDefinition.BondType.ToString();
            }
        }

        public void ClearDisplay()
        {
            if (resultRoot != null)
            {
                resultRoot.SetActive(true);
            }

            if (nameText != null)
            {
                nameText.text = emptyNameText;
            }

            if (formulaText != null)
            {
                formulaText.text = emptyFormulaText;
            }

            if (bondText != null)
            {
                bondText.text = emptyBondText;
            }
        }
    }
}