using UnityEngine;

namespace VRMolecularLab.Molecules
{
    public class LabResetController : MonoBehaviour
    {
        [SerializeField] private ReactionZone reactionZone;

        public void ResetLab()
        {
            if (reactionZone == null)
            {
                Debug.LogWarning("LabResetController has no ReactionZone assigned.", this);
                return;
            }

            reactionZone.ResetReactionState();
        }
    }
}