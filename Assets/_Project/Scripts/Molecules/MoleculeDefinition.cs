using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.Molecules
{
    [CreateAssetMenu(
        fileName = "MoleculeDefinition",
        menuName = "VR Molecular Lab/Molecules/Molecule Definition")]
    public class MoleculeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string moleculeName;
        [SerializeField] private string formula;

        [Header("Chemistry")]
        [SerializeField] private BondType bondType = BondType.Single;
        [SerializeField] private List<AtomCountRequirement> requiredAtoms = new();

        [Header("Presentation")]
        [SerializeField] private GameObject moleculePrefab;
        [TextArea]
        [SerializeField] private string description;

        public string MoleculeName => moleculeName;
        public string Formula => formula;
        public BondType BondType => bondType;
        public IReadOnlyList<AtomCountRequirement> RequiredAtoms => requiredAtoms;
        public GameObject MoleculePrefab => moleculePrefab;
        public string Description => description;

        public bool Matches(Dictionary<AtomType, int> atomCounts)
        {
            if (requiredAtoms == null || requiredAtoms.Count == 0)
            {
                return false;
            }

            int requiredTotal = 0;
            foreach (AtomCountRequirement requirement in requiredAtoms)
            {
                if (requirement == null || requirement.Count <= 0)
                {
                    continue;
                }

                requiredTotal += requirement.Count;

                if (!atomCounts.TryGetValue(requirement.AtomType, out int actualCount))
                {
                    return false;
                }

                if (actualCount != requirement.Count)
                {
                    return false;
                }
            }

            int providedTotal = 0;
            foreach (KeyValuePair<AtomType, int> pair in atomCounts)
            {
                providedTotal += pair.Value;
            }

            return providedTotal == requiredTotal;
        }
    }
}