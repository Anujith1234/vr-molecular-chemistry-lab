using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using VRMolecularLab.Data;
using VRMolecularLab.UI;
using VRMolecularLab.XR;
using VRMolecularLab.Audio;

namespace VRMolecularLab.Molecules
{
    public class ReactionZone : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private MoleculeDatabase moleculeDatabase;
        [SerializeField] private BoxCollider reactionTrigger;

        [Header("Display")]
        [SerializeField] private MoleculeResultDisplay moleculeResultDisplay;
        [SerializeField] private MoleculeVisualPresenter moleculeVisualPresenter;
        [SerializeField] private MoleculeLibraryDisplay moleculeLibraryDisplay;

        [Header("Audio")]
        [SerializeField] private LabAudioManager labAudioManager;

        [Header("Debug")]
        [SerializeField] private bool logMatches = true;

        private readonly HashSet<AtomController> atomsInZone = new();
        private readonly List<AtomResettable> consumedAtoms = new();

        private bool reactionLocked;
        public IReadOnlyCollection<AtomController> AtomsInZone => atomsInZone;

        private void Awake()
        {
            if (reactionTrigger == null)
            {
                reactionTrigger = GetComponent<BoxCollider>();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (reactionLocked)
            {
                return;
            }

            if (!other.TryGetComponent(out AtomController atomController))
            {
                return;
            }

            atomsInZone.Add(atomController);

            if (logMatches)
            {
                Debug.Log($"Atom entered reaction zone: {atomController.AtomType}", this);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (reactionLocked) 
            {
                return;
            }

            if (!other.TryGetComponent(out AtomController atomController))
            {
                return;
            }

            atomsInZone.Remove(atomController);

            if (logMatches)
            {
                Debug.Log($"Atom exited reaction zone: {atomController.AtomType}", this);
            }
        }
        public void TryFormCurrentMolecule()
        {
            if (reactionLocked)
            {
                Debug.Log("ReactionZone is locked.Reset before forming a new molecule.", this);
                return;
            }

            RefreshAtomsInZoneFromTrigger();

            if (atomsInZone.Count == 0)
            {
                Debug.Log("No atoms found inside the reaction chamber.", this);
                return;
            }

            EvaluateCurrentContents();
        }
        public MoleculeDefinition EvaluateCurrentContents()
        {
            if (reactionLocked)
            {
                return null;
            }

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

            if (match != null)
            {
                ConsumeCurrentAtoms();

                if (labAudioManager != null)
                {
                    labAudioManager.PlayFormSuccess();
                }

                if (logMatches)
                {
                    Debug.Log($"Valid molecule detected: {match.MoleculeName} ({match.Formula})", this);
                }
            }
            else if (atomCounts.Count > 0 && logMatches)
            {
                Debug.Log("Current atom set does not match a valid molecule.", this);
            }

            return match;
        }

        public void ResetReactionState()
        {
            Debug.Log($"Restoring {consumedAtoms.Count} atoms");

            foreach (AtomResettable atomresettable in consumedAtoms)
            {
                if (atomresettable != null)
                {
                    atomresettable.RestoreToInitialState();
                }
            }

            consumedAtoms.Clear();
            atomsInZone.Clear();
            reactionLocked = false;

            if (moleculeResultDisplay != null)
            {
                moleculeResultDisplay.ClearDisplay();
            }

            if (moleculeVisualPresenter != null)
            {
                moleculeVisualPresenter.ClearVisual();
            }
        }

        private void RefreshAtomsInZoneFromTrigger()
        {
            atomsInZone.Clear();

            if (reactionTrigger == null)
            {
                Debug.LogWarning("ReactionZone has no reactionTrigger assigned.", this);
                return;
            }

            Vector3 worldCenter = reactionTrigger.transform.TransformPoint(reactionTrigger.center);
            Vector3 worldHalfExtents = Vector3.Scale(reactionTrigger.size * 0.5f, reactionTrigger.transform.lossyScale);

            Collider[] overlaps = Physics.OverlapBox(
                worldCenter,
                worldHalfExtents,
                reactionTrigger.transform.rotation,
                ~0,
                QueryTriggerInteraction.Ignore);

            HashSet<AtomController> uniqueAtoms = new();

            foreach (Collider overlap in overlaps)
            {
                AtomController atom = overlap.GetComponent<AtomController>();

                if (atom == null)
                {
                    atom = overlap.GetComponentInParent<AtomController>();
                }

                if (atom != null)
                {
                    uniqueAtoms.Add(atom);
                }
            }

            atomsInZone.UnionWith(uniqueAtoms);
        }
        private void ConsumeCurrentAtoms()
        {
            consumedAtoms.Clear();

            Debug.Log($"Consuming {atomsInZone.Count} atoms");

            foreach (AtomController atom in atomsInZone)
            {
                if (atom == null)
                {
                    continue;
                }

                if (atom.TryGetComponent(out AtomResettable atomResettable))
                {
                    consumedAtoms.Add(atomResettable);
                    atomResettable.Consume();
                }
            }

            atomsInZone.Clear();
            reactionLocked = true;
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