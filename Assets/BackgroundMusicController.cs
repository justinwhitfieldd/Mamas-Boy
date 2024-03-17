using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;

namespace AudioHelper.AudioControl
{
    public class BackgroundMusicController : MonoBehaviour
    {
        public IList<AudioClipAdvanced> SourceAsset;
        public AudioClipAdvanced[] SourceAssetArray = new AudioClipAdvanced[0];
        public AudioSource SourceComponent;
        public GameObject laser;
        /// <summary>
        /// The currently playing track, represented by an integer.
        /// </summary>
        private int NowPlaying = 0;
        /// <summary>
        /// The ammount of tracks in the playlist or array.
        /// </summary>
        private int Count;
        /// <summary>
        /// <para>The track to start with. Default is 0.</para>
        /// <para>Used with "InitializePlaylist(int TrackNumber)".</para>
        /// </summary>
        public int StartFrom = 0;
        /// <summary>
        /// The total length of the current track.
        /// </summary>
        private float MaxTime;
        //private float Time; // Don't use, obsolete.

        /// <summary>
        /// Tells the thread if playback has already begun.
        /// </summary>
        private bool PlaylistInitialized = false;

        /// <summary>
        /// Begins playing from the playlist/array, in order.
        /// </summary>
        public void InitializePlaylist()
        {
            if (SourceComponent.isPlaying)
            {
                PlaylistInitialized = false;
                SourceComponent.Stop();
            }
            NowPlaying = 0;
            SourceAsset = SourceAssetArray;
            MaxTime = SourceAsset[NowPlaying].source.length;
            Count = SourceAssetArray.Count<AudioClipAdvanced>();

            SourceComponent.clip = SourceAsset[NowPlaying].source;
            SourceComponent.loop = SourceAsset[NowPlaying].Loop;
            SourceComponent.Play();

            PlaylistInitialized = true;
        }

        /// <summary>
        /// Begins playing from the playlist/array, in order.
        /// </summary>
        /// <param name="TrackNumber">The song from the array to start with. Default is 0 (first track).</param>
        public void InitializePlaylist(int TrackNumber)
        {
            if (SourceComponent.isPlaying)
            {
                PlaylistInitialized = false;
                SourceComponent.Stop();
            }
            NowPlaying = TrackNumber;
            SourceAsset = SourceAssetArray;
            MaxTime = SourceAsset[NowPlaying].source.length;
            Count = SourceAssetArray.Count<AudioClipAdvanced>();

            SourceComponent.clip = SourceAsset[NowPlaying].source;
            SourceComponent.loop = SourceAsset[NowPlaying].Loop;
            SourceComponent.Play();

            PlaylistInitialized = true;
        }

        /// <summary>
        /// Checks if the song is over, and begins the next song if it is.
        /// </summary>
        public void Advance()
        {
            if (SourceComponent.loop || NowPlaying == Count) { return; } // Alternatively, inside a loop, use 'break;' to cancel or exit the playlist completely.
            else if (SourceComponent.time >= MaxTime)
            {
                SourceComponent.Stop();
                NowPlaying++;
                SourceComponent.clip = SourceAsset[NowPlaying].source;
                SourceComponent.loop = SourceAsset[NowPlaying].Loop;
                SourceComponent.Play();
                laser.SetActive(true);
            }
        }

        void Start()
        {
            // Tells the thread to start on a certain track, if specified.
            if (StartFrom <= 0) { InitializePlaylist(); } else { InitializePlaylist(StartFrom); }
        }

        void Update()
        {
            if (PlaylistInitialized)
            {
                try
                {
                    Advance();
                }
                catch (Exception err)
                {
                    Debug.LogException(err);
                }
            }
        }
    }
}
