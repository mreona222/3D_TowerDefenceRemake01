using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Template.Manager;

namespace TowerDefenseRemake.Manager
{
    public class AudioManager : ManagerBase<AudioManager>
    {
        public enum BGMEnum
        {
            a,
        }

        public enum SEEnum
        {
            a,
        }


        [Header("AudioClip")]
        [SerializeField] public AudioClip[] bgmList;
        [SerializeField] public AudioClip[] seList;

        [Header("AudioSource")]
        [SerializeField] public AudioSource bgmSource;
        [SerializeField] public AudioSource seSource;

        [Header("AudioMixer")]
        [SerializeField] AudioMixer _mixer;


        protected override void Init()
        {
            base.Init();
            GameManager.OnGameStateChanged += GameStateLinkBGM;
        }


        private void GameStateLinkBGM(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.Title:
                    BGMChange(bgmSource, BGMEnum.a);
                    break;
                case GameManager.GameState.Menu:
                    BGMChange(bgmSource, BGMEnum.a);
                    break;
            }
        }

        public void BGMChange(AudioSource source, BGMEnum number)
        {
            if (source.clip != bgmList[(int)number])
            {
                source.clip = bgmList[(int)number];
                source.loop = true;
                source.Play();
            }
        }

        public void SEPlay(AudioSource source, SEEnum number)
        {
            source.PlayOneShot(seList[(int)number]);
        }
    }
}