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