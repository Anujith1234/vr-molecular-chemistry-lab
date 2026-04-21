using UnityEngine;
using VRMolecularLab.Audio;

namespace VRMolecularLab.Molecules
{
    public class LabResetController : MonoBehaviour
    {
        [SerializeField] private ReactionZone reactionZone;
        [SerializeField] private LabAudioManager labAudioManager;

        public void ResetLab()
        {
            if (reactionZone == null)
            {
                Debug.LogWarning("LabResetController has no ReactionZone assigned.", this);
                return;
            }

            reactionZone.ResetReactionState();

            if (labAudioManager != null)
            {
                labAudioManager.PlayResetLab();
            }
        }
    }
}