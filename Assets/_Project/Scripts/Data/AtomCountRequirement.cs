using System;
using UnityEngine;

namespace VRMolecularLab.Data
{
    [Serializable]
    public class AtomCountRequirement
    {
        [SerializeField] private AtomType atomType;
        [SerializeField] private int count = 1;

        public AtomType AtomType => atomType;
        public int Count => count;
    }
}