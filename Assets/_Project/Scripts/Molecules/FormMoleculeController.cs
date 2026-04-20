using UnityEngine;

namespace VRMolecularLab.Molecules
{
    public class FormMoleculeController : MonoBehaviour
    {
        [SerializeField] private ReactionZone reactionZone;

        public void FormMolecule()
        {
            if (reactionZone == null)
            {
                Debug.LogWarning("FormMoleculeController has no ReactionZone assigned.", this);
                return;
            }

            reactionZone.TryFormCurrentMolecule();
        }
    }
}