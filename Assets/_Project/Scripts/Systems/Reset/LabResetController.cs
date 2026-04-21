using UnityEngine;
using VRMolecularLab.Audio;

namespace VRMolecularLab.Molecules
{
    public class LabResetController : MonoBehaviour
    {
        [Tooltip("Reaction zone that owns the current chamber state and consumed atoms.")]
        [SerializeField] private ReactionZone reactionZone;

        [Tooltip("Audio manager used for reset feedback.")]
        [SerializeField] private LabAudioManager labAudioManager;

        public void ResetLab()
        {
            if (reactionZone == null)
            {
                Debug.LogWarning("LabResetController is missing its ReactionZone reference.", this);
                return;
            }

            reactionZone.ResetReactionState();
            labAudioManager?.PlayResetLab();
        }
    }
}