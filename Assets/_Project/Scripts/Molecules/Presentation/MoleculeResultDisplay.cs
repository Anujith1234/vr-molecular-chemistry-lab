using TMPro;
using UnityEngine;

namespace VRMolecularLab.Molecules
{
    public class MoleculeResultDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject resultRoot;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text formulaText;
        [SerializeField] private TMP_Text bondText;

        [Header("Fallback Text")]
        [SerializeField] private string emptyNameText = "No Molecule";
        [SerializeField] private string emptyFormulaText = "--";
        [SerializeField] private string emptyBondText = "--";

        private void Awake()
        {
            SetDisplay(emptyNameText, emptyFormulaText, emptyBondText);
            SetRootVisible(true);
        }

        public void ShowMolecule(MoleculeDefinition moleculeDefinition)
        {
            if (moleculeDefinition == null)
            {
                ClearDisplay();
                return;
            }

            SetDisplay(
                moleculeDefinition.MoleculeName,
                moleculeDefinition.Formula,
                moleculeDefinition.BondType.ToString()
            );

            SetRootVisible(true);
        }

        public void ClearDisplay()
        {
            SetDisplay(emptyNameText, emptyFormulaText, emptyBondText);
            SetRootVisible(true);
        }

        private void SetDisplay(string name, string formula, string bond)
        {
            if (nameText != null)
                nameText.text = name;

            if (formulaText != null)
                formulaText.text = formula;

            if (bondText != null)
                bondText.text = bond;
        }

        private void SetRootVisible(bool visible)
        {
            if (resultRoot != null)
            {
                resultRoot.SetActive(visible);
            }
        }
    }
}