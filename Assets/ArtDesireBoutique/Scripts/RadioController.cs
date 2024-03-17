using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoundFXs
{

public class RadioController : MonoBehaviour
{
    [Header("List of Tracks")]
    [SerializeField] private Track[] audioTracks;
    private int trackIndex;

    [Header("Text UI")]
    [SerializeField] private Text trackTextUI;

    private AudioSource radioAudioSource;

    private void Start()
    {
        radioAudioSource = GetComponent<AudioSource>();

        trackIndex = 0;
        radioAudioSource.clip = audioTracks[trackIndex].trackAudioClip;
        trackTextUI.text = audioTracks[trackIndex].name;
    }

    public void SkipForwardButton()
    {
        if (trackIndex < audioTracks.Length - 1)
        {
            trackIndex++;
            UpdateTrack(trackIndex);
        }
    }
        public void SkipBackwardsButton()
        {
            if (trackIndex >= 1)
            {
                trackIndex--;
                UpdateTrack(trackIndex);
            }

        }

        private void UpdateTrack(int index)
        {
            radioAudioSource.clip = audioTracks[index].trackAudioClip;
            trackTextUI.text = audioTracks[index].name;
        }

        public void AudioVolume(float volume)
        {
            radioAudioSource.volume = volume;
        }

        public void PlayAudio()
        {
            radioAudioSource.Play();
        }

        public void PauseAudio()
        {
            radioAudioSource.Pause();
        }

        public void StopAudio()
        {
            radioAudioSource.Stop();
        }
    }

}
