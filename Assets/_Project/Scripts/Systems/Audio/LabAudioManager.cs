using UnityEngine;

namespace VRMolecularLab.Audio
{
    public class LabAudioManager : MonoBehaviour
    {
        [Header("Audio Source")]
        [Tooltip("Shared AudioSource used for short lab feedback sounds.")]
        [SerializeField] private AudioSource sfxSource;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip formSuccessClip;
        [SerializeField] private AudioClip resetLabClip;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f)] private float formSuccessVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float resetLabVolume = 0.8f;

        public void PlayFormSuccess()
        {
            PlayClip(formSuccessClip, formSuccessVolume, "form success");
        }

        public void PlayResetLab()
        {
            PlayClip(resetLabClip, resetLabVolume, "reset lab");
        }

        private void PlayClip(AudioClip clip, float volume, string clipContext)
        {
            if (sfxSource == null)
            {
                Debug.LogWarning("LabAudioManager is missing its AudioSource reference.", this);
                return;
            }

            if (clip == null)
            {
                Debug.LogWarning($"LabAudioManager is missing the {clipContext} clip.", this);
                return;
            }

            sfxSource.PlayOneShot(clip, volume);
        }
    }
}