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

        public MoleculeDefinition FindMatchingMolecule(IReadOnlyDictionary<AtomType, int> atomCounts)
        {
            if (atomCounts == null || atomCounts.Count == 0)
            {
                return null;
            }

            if (moleculeDefinitions == null || moleculeDefinitions.Count == 0)
            {
                return null;
            }

            foreach (MoleculeDefinition molecule in moleculeDefinitions)
            {
                if (molecule == null)
                {
                    continue;
                }

                if (molecule.Matches(atomCounts))
                {
                    return molecule;
                }
            }

            return null;
        }
    }
}