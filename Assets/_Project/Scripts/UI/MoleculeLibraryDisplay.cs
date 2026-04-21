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
        [SerializeField] private string discoveredLabel = "[Discovered]";
        [SerializeField] private string lockedLabel = "[Locked]";

        private readonly HashSet<MoleculeDefinition> discoveredMolecules = new();

        private void Awake()
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
        public bool HasDiscoveredAllMolecules()
        {
            if (moleculeDatabase == null || moleculeDatabase.MoleculeDefinitions == null)
            {
                return false;
            }

            foreach (MoleculeDefinition molecule in moleculeDatabase.MoleculeDefinitions)
            {
                if (molecule == null)
                {
                    continue;
                }

                if (!discoveredMolecules.Contains(molecule))
                {
                    return false;
                }
            }

            return true;
        }
        public void RefreshDisplay()
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (listText == null)
            {
                Debug.LogWarning("MoleculeLibraryDisplay is missing its list text reference.", this);
                return;
            }

            if (moleculeDatabase == null)
            {
                Debug.LogWarning("MoleculeLibraryDisplay is missing its MoleculeDatabase reference.", this);
                listText.text = string.Empty;
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
                string stateLabel = isDiscovered ? discoveredLabel : lockedLabel;

                builder.AppendLine($"{stateLabel} {molecule.MoleculeName} ({molecule.Formula})");
            }

            listText.text = builder.ToString();
        }

        public void ClearDiscoveredState()
        {
            discoveredMolecules.Clear();
            RefreshDisplay();
        }
    }
}