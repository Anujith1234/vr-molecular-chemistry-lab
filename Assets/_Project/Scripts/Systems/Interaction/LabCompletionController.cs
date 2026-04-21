using TMPro;
using UnityEngine;
using VRMolecularLab.UI;

namespace VRMolecularLab.Molecules
{
    public class LabCompletionController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject completionRoot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;

        [Header("Text")]
        [SerializeField] private string completionTitle = "Lab Complete";
        [SerializeField] private string completionBody = "All core molecules discovered.";

        [Header("Dependencies")]
        [SerializeField] private ReactionZone reactionZone;
        [SerializeField] private MoleculeLibraryDisplay moleculeLibraryDisplay;

        private void Awake()
        {
            HideCompletion();
        }

        public void ShowCompletion()
        {
            if (titleText != null)
            {
                titleText.text = completionTitle;
            }

            if (bodyText != null)
            {
                bodyText.text = completionBody;
            }

            if (completionRoot != null)
            {
                completionRoot.SetActive(true);
            }
        }

        public void HideCompletion()
        {
            if (completionRoot != null)
            {
                completionRoot.SetActive(false);
            }
        }

        public void StartOver()
        {
            reactionZone?.ResetReactionState();
            moleculeLibraryDisplay?.ClearDiscoveredState();
            HideCompletion();
        }
    }
}