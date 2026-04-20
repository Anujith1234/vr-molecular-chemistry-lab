using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.Data;
using VRMolecularLab.XR;
using VRMolecularLab.UI;

namespace VRMolecularLab.Molecules
{
    public class ReactionZone : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private MoleculeDatabase moleculeDatabase;

        [Header("Display")]
        [SerializeField] private MoleculeResultDisplay moleculeResultDisplay;
        [SerializeField] private MoleculeVisualPresenter moleculeVisualPresenter;
        [SerializeField] private MoleculeLibraryDisplay moleculeLibraryDisplay;

        [Header("Debug")]
        [SerializeField] private bool logMatches = true;

        private readonly HashSet<AtomController> atomsInZone = new();

        public IReadOnlyCollection<AtomController> AtomsInZone => atomsInZone;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out AtomController atomController))
            {
                return;
            }

            atomsInZone.Add(atomController);
            EvaluateCurrentContents();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out AtomController atomController))
            {
                return;
            }

            atomsInZone.Remove(atomController);
            EvaluateCurrentContents();
        }

        public MoleculeDefinition EvaluateCurrentContents()
        {
            if (moleculeDatabase == null)
            {
                Debug.LogWarning("ReactionZone has no MoleculeDatabase assigned.", this);
                return null;
            }

            Dictionary<AtomType, int> atomCounts = BuildAtomCounts();

            MoleculeDefinition match = moleculeDatabase.FindMatchingMolecule(atomCounts);

            if (moleculeResultDisplay != null)
            {
                if (match != null)
                {
                    moleculeResultDisplay.ShowMolecule(match);
                }
                else
                {
                    moleculeResultDisplay.ClearDisplay();
                }
            }

            if (match != null && moleculeLibraryDisplay != null)
            {
                moleculeLibraryDisplay.RegisterDiscoveredMolecule(match);
            }

            if (moleculeVisualPresenter != null)
            {
                if (match != null)
                {
                    moleculeVisualPresenter.ShowMolecule(match);
                }
                else
                {
                    moleculeVisualPresenter.ClearVisual();
                }
            }

            if (logMatches)
            {
                if (match != null)
                {
                    Debug.Log($"Valid molecule detected: {match.MoleculeName} ({match.Formula})", this);
                }
                else if (atomCounts.Count > 0)
                {
                    Debug.Log("Current atom set does not match a valid molecule.", this);
                }
            }

            return match;
        }

        private Dictionary<AtomType, int> BuildAtomCounts()
        {
            Dictionary<AtomType, int> atomCounts = new();

            foreach (AtomController atom in atomsInZone)
            {
                if (atom == null)
                {
                    continue;
                }

                if (!atomCounts.ContainsKey(atom.AtomType))
                {
                    atomCounts[atom.AtomType] = 0;
                }

                atomCounts[atom.AtomType]++;
            }

            return atomCounts;
        }
    }
}