using Photon.Bolt;
using UnityEngine;
using UnityEngine.Video;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class NetworkVideo : EntityBehaviour<IVideoState>
    {
        public VideoClip[] videoClips;

        public VideoPlayer videoPlayer;
        public GameObject pauseIcon;

        public override void Attached()
        {
            state.AddCallback("Index", ChangeVideo);
            state.AddCallback("Skip", Skip);
            state.AddCallback("Pause", Pause);

            videoPlayer.time = state.Time;
        }

        private void Update()
        {
            if (!entity.IsAttached || !entity.IsOwner)
            {
                return;
            }

            state.Time = (float)videoPlayer.time;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                state.Skip = state.Time - 10;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                state.Skip = state.Time + 10;
            }

            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                state.Pause = !state.Pause;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                state.Time = 0;

                if (state.Index < videoClips.Length - 1)
                {
                    state.Index++;
                }
                else
                {
                    state.Index = 0;
                }
            }
        }

        private void Skip()
        {
            videoPlayer.time = state.Skip;
        }

        private void Pause()
        {
            pauseIcon.SetActive(state.Pause);

            if (state.Pause)
            {
                videoPlayer.Pause();
            }
            else if (!state.Pause)
            {
                videoPlayer.Play();
            }

            if (!entity.IsOwner)
            {
                videoPlayer.time = state.Time;
            }
        }

        private void ChangeVideo()
        {
            if (state.Index >= 0 && state.Index < videoClips.Length && videoClips[state.Index] != null)
            {
                videoPlayer.clip = videoClips[state.Index];

                if (!entity.IsOwner)
                {
                    Pause();
                }
            }
        }
    }
}