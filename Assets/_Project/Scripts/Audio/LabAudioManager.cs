using UnityEngine;

namespace VRMolecularLab.Audio
{
    public class LabAudioManager : MonoBehaviour
    {
        [Header("Audio Source")]
        [SerializeField] private AudioSource sfxSource;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip formSuccessClip;
        [SerializeField] private AudioClip resetLabClip;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f)] private float formSuccessVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float resetLabVolume = 0.8f;

        public void PlayFormSuccess()
        {
            PlayOneShot(formSuccessClip, formSuccessVolume);
        }

        public void PlayResetLab()
        {
            PlayOneShot(resetLabClip, resetLabVolume);
        }

        private void PlayOneShot(AudioClip clip, float volume)
        {
            if (sfxSource == null)
            {
                Debug.LogWarning("LabAudioManager has no AudioSource assigned.", this);
                return;
            }

            if (clip == null)
            {
                Debug.LogWarning("LabAudioManager was asked to play a null AudioClip.", this);
                return;
            }

            sfxSource.PlayOneShot(clip, volume);
        }
    }
}