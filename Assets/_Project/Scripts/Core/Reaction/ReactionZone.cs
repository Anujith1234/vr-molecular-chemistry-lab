using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.Data;
using VRMolecularLab.UI;
using VRMolecularLab.XR;
using VRMolecularLab.Audio;

namespace VRMolecularLab.Molecules
{
    public class ReactionZone : MonoBehaviour
    {
        #region Inspector

        [Header("Data")]
        [SerializeField] private MoleculeDatabase moleculeDatabase;
        [SerializeField] private BoxCollider reactionTrigger;

        [Header("Presentation")]
        [SerializeField] private MoleculeResultDisplay resultDisplay;
        [SerializeField] private MoleculeVisualPresenter visualPresenter;
        [SerializeField] private MoleculeLibraryDisplay libraryDisplay;

        [Header("Audio")]
        [SerializeField] private LabAudioManager audioManager;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;

        #endregion

        #region State

        private readonly HashSet<AtomController> atomsInZone = new();
        private readonly List<AtomResettable> consumedAtoms = new();

        private bool isReactionLocked;

        public IReadOnlyCollection<AtomController> AtomsInZone => atomsInZone;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (reactionTrigger == null)
            {
                reactionTrigger = GetComponent<BoxCollider>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isReactionLocked) return;

            if (TryGetAtom(other, out var atom))
            {
                atomsInZone.Add(atom);
                Log($"Atom entered: {atom.AtomType}");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isReactionLocked) return;

            if (TryGetAtom(other, out var atom))
            {
                atomsInZone.Remove(atom);
                Log($"Atom exited: {atom.AtomType}");
            }
        }

        #endregion

        #region Public API

        public void TryFormCurrentMolecule()
        {
            if (isReactionLocked)
            {
                Log("Reaction locked. Reset required.");
                return;
            }

            RefreshAtoms();

            if (atomsInZone.Count == 0)
            {
                Log("No atoms in reaction zone.");
                return;
            }

            var match = FindMatchingMolecule();

            HandleReactionResult(match);
        }

        public void ResetReactionState()
        {
            RestoreConsumedAtoms();

            atomsInZone.Clear();
            isReactionLocked = false;

            resultDisplay?.ClearDisplay();
            visualPresenter?.ClearVisual();

            audioManager?.PlayResetLab();
        }

        #endregion

        #region Core Logic

        private MoleculeDefinition FindMatchingMolecule()
        {
            if (moleculeDatabase == null)
            {
                Debug.LogWarning("Missing MoleculeDatabase.", this);
                return null;
            }

            var atomCounts = BuildAtomCounts();
            return moleculeDatabase.FindMatchingMolecule(atomCounts);
        }

        private void HandleReactionResult(MoleculeDefinition match)
        {
            UpdateUI(match);
            UpdateVisuals(match);

            if (match == null)
            {
                Log("No valid molecule match.");
                return;
            }

            libraryDisplay?.RegisterDiscoveredMolecule(match);

            ConsumeAtoms();

            audioManager?.PlayFormSuccess();

            Log($"Molecule formed: {match.MoleculeName}");
        }

        #endregion

        #region Helpers

        private void UpdateUI(MoleculeDefinition match)
        {
            if (resultDisplay == null) return;

            if (match != null)
                resultDisplay.ShowMolecule(match);
            else
                resultDisplay.ClearDisplay();
        }

        private void UpdateVisuals(MoleculeDefinition match)
        {
            if (visualPresenter == null) return;

            if (match != null)
                visualPresenter.ShowMolecule(match);
            else
                visualPresenter.ClearVisual();
        }

        private void RefreshAtoms()
        {
            atomsInZone.Clear();

            if (reactionTrigger == null)
            {
                Debug.LogWarning("Missing reactionTrigger.", this);
                return;
            }

            Vector3 center = reactionTrigger.transform.TransformPoint(reactionTrigger.center);
            Vector3 halfExtents = Vector3.Scale(reactionTrigger.size * 0.5f, reactionTrigger.transform.lossyScale);

            var overlaps = Physics.OverlapBox(
                center,
                halfExtents,
                reactionTrigger.transform.rotation,
                ~0,
                QueryTriggerInteraction.Ignore
            );

            HashSet<AtomController> uniqueAtoms = new();

            foreach (var col in overlaps)
            {
                if (TryGetAtom(col, out var atom))
                {
                    uniqueAtoms.Add(atom);
                }
            }

            atomsInZone.UnionWith(uniqueAtoms);
        }

        private void ConsumeAtoms()
        {
            consumedAtoms.Clear();

            foreach (var atom in atomsInZone)
            {
                if (atom == null) continue;

                if (atom.TryGetComponent(out AtomResettable resettable))
                {
                    consumedAtoms.Add(resettable);
                    resettable.Consume();
                }
            }

            atomsInZone.Clear();
            isReactionLocked = true;
        }

        private void RestoreConsumedAtoms()
        {
            foreach (var atom in consumedAtoms)
            {
                atom?.RestoreToInitialState();
            }

            consumedAtoms.Clear();
        }

        private Dictionary<AtomType, int> BuildAtomCounts()
        {
            var counts = new Dictionary<AtomType, int>();

            foreach (var atom in atomsInZone)
            {
                if (atom == null) continue;

                if (!counts.ContainsKey(atom.AtomType))
                    counts[atom.AtomType] = 0;

                counts[atom.AtomType]++;
            }

            return counts;
        }

        private bool TryGetAtom(Collider col, out AtomController atom)
        {
            if (col.TryGetComponent(out atom)) return true;
            return col.GetComponentInParent<AtomController>() != null
                ? (atom = col.GetComponentInParent<AtomController>()) != null
                : false;
        }

        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[ReactionZone] {message}", this);
            }
        }

        #endregion
    }
}