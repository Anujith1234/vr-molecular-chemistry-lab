using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using VRMolecularLab.Molecules;

namespace VRMolecularLab.UI
{
    public class MoleculeLibraryDisplay : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private MoleculeDatabase moleculeDatabase;

        [Header("UI References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text listText;

        [Header("Text")]
        [SerializeField] private string title = "Molecule Library";

        private readonly HashSet<MoleculeDefinition> discoveredMolecules = new();

        private void Start()
        {
            RefreshDisplay();
        }

        public void RegisterDiscoveredMolecule(MoleculeDefinition moleculeDefinition)
        {
            if (moleculeDefinition == null)
            {
                return;
            }

            if (discoveredMolecules.Add(moleculeDefinition))
            {
                RefreshDisplay();
            }
        }

        public void RefreshDisplay()
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (listText == null || moleculeDatabase == null)
            {
                return;
            }

            StringBuilder builder = new();

            foreach (MoleculeDefinition molecule in moleculeDatabase.MoleculeDefinitions)
            {
                if (molecule == null)
                {
                    continue;
                }

                bool isDiscovered = discoveredMolecules.Contains(molecule);
                string stateText = isDiscovered ? "[Discovered]" : "[Locked]";

                builder.AppendLine($"{stateText} {molecule.MoleculeName} ({molecule.Formula})");
            }

            listText.text = builder.ToString();
        }
    }
}