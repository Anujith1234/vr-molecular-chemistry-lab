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

        [Header("Completion")]
        [SerializeField] private LabCompletionController completionController;

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
                atom.SetBondingPreview(true);
                Log($"Atom entered: {atom.AtomType}");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isReactionLocked) return;

            if (TryGetAtom(other, out var atom))
            {
                atomsInZone.Remove(atom);
                atom.SetBondingPreview(false);
                Log($"Atom exited: {atom.AtomType}");
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (isReactionLocked) return;

            if (TryGetAtom(other, out var atom))
            {
                atomsInZone.Add(atom);
                atom.SetBondingPreview(true);
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
                resultDisplay?.ShowInvalidCombination();
                visualPresenter?.ClearVisual();
                audioManager?.PlayInvalidForm();
                Log("No atoms in reaction zone.");
                return;
            }

            var match = FindMatchingMolecule();

            HandleReactionResult(match);
        }

        public void ResetReactionState()
        {
            RestoreConsumedAtoms();

            foreach (var atom in atomsInZone)
            {
                if (atom != null)
                {
                    atom.SetBondingPreview(false);
                }
            }

            atomsInZone.Clear();
            isReactionLocked = false;

            resultDisplay?.ClearDisplay();
            visualPresenter?.ClearVisual();
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
            if (match == null)
            {
                resultDisplay?.ShowInvalidCombination();
                visualPresenter?.ClearVisual();
                audioManager?.PlayInvalidForm();
                Log("No valid molecule match.");
                return;
            }

            UpdateUI(match);
            UpdateVisuals(match);

            libraryDisplay?.RegisterDiscoveredMolecule(match);

            bool allMoleculesDiscovered = libraryDisplay != null && libraryDisplay.HasDiscoveredAllMolecules();

            ConsumeAtoms();

            audioManager?.PlayFormSuccess();

            if (allMoleculesDiscovered)
            {
                completionController?.ShowCompletion();
            }

            Log($"Molecule formed: {match.MoleculeName}");
        }

        #endregion

        #region Helpers

        private void UpdateUI(MoleculeDefinition match)
        {
            if (resultDisplay == null) return;

            resultDisplay.ShowMolecule(match);
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
            foreach (var atom in atomsInZone)
            {
                if (atom != null)
                {
                    atom.SetBondingPreview(false);
                }
            }

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

            foreach (var atom in atomsInZone)
            {
                if (atom != null)
                {
                    atom.SetBondingPreview(true);
                }
            }
        }

        private void ConsumeAtoms()
        {
            consumedAtoms.Clear();

            foreach (var atom in atomsInZone)
            {
                if (atom == null) continue;

                atom.SetBondingPreview(false);

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