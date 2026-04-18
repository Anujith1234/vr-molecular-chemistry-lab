using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.Molecules
{
    [CreateAssetMenu(
        fileName = "MoleculeDatabase",
        menuName = "VR Molecular Lab/Molecules/Molecule Database")]
    public class MoleculeDatabase : ScriptableObject
    {
        [SerializeField] private List<MoleculeDefinition> moleculeDefinitions = new();

        public IReadOnlyList<MoleculeDefinition> MoleculeDefinitions => moleculeDefinitions;

        public MoleculeDefinition FindMatchingMolecule(Dictionary<AtomType, int> atomCounts)
        {
            if (atomCounts == null || atomCounts.Count == 0)
            {
                return null;
            }

            foreach (MoleculeDefinition moleculeDefinition in moleculeDefinitions)
            {
                if (moleculeDefinition == null)
                {
                    continue;
                }

                if (moleculeDefinition.Matches(atomCounts))
                {
                    return moleculeDefinition;
                }
            }

            return null;
        }
    }
}