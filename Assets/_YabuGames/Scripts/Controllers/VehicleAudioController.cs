using System;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class VehicleAudioController: MonoBehaviour
    {
        [SerializeField] private AudioClip acceleratingSound, cutterSound, skidSound;
        [SerializeField] private AudioSource effectSource, mainSource;

        private bool _isSkidding;
        private float _currentPitch;
        private AudioListener _listener;

        private void Awake()
        {
            _listener = GetComponent<AudioListener>();
        }

        #region Subscribtions

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            LevelSignals.Instance.OnSkillPanel += Mute;
            CoreGameSignals.Instance.OnLevelFail += LevelEnd;
            CoreGameSignals.Instance.OnLevelWin += LevelEnd;
            // CoreGameSignals.Instance.OnLevelFail += Lose;
            // CoreGameSignals.Instance.OnLevelWin += Lose;
        }

        private void UnSubscribe()
        {
            LevelSignals.Instance.OnSkillPanel -= Mute;
            CoreGameSignals.Instance.OnLevelFail -= LevelEnd;
            CoreGameSignals.Instance.OnLevelWin -= LevelEnd;
            // CoreGameSignals.Instance.OnLevelFail -= Lose;
            // CoreGameSignals.Instance.OnLevelWin -= Lose;
        }

        #endregion

        private void Lose()
        {
            GetComponent<AudioListener>().enabled = false;
        }
        
        private void LevelEnd()
        {
            Mute(true);
        }
        private void Mute(bool isMuted)
        {
            // if (isMuted)
            // {
            //     effectSource.Pause();
            //     mainSource.Pause();
            // }
            // else
            // {
            //     effectSource.UnPause();
            //     mainSource.UnPause();
            // }
             _listener.enabled = !isMuted;
        }
        public void BeginSkidding()
        {
            if (_isSkidding) 
                return;
            
            _isSkidding = true;
            _currentPitch = mainSource.pitch;
            mainSource.DOKill();
            if (!effectSource.isPlaying)
            {
                effectSource.clip = skidSound;
                effectSource.Play();
            }
            
        }

        public void EndSkidding()
        {
            if (!_isSkidding) 
                return;
            _isSkidding = false;
            if (!mainSource.isPlaying)
            {
                mainSource.Play();
            }
            mainSource.DOKill();
            if (_currentPitch>0)
            {
                mainSource.pitch = _currentPitch;
            }
            else
            {
                mainSource.pitch = .7f;
            }
            effectSource.Stop();
            mainSource.clip = acceleratingSound;
            mainSource.DOPitch(2, 3).SetLoops(20, LoopType.Restart);
        }
        
    }
}