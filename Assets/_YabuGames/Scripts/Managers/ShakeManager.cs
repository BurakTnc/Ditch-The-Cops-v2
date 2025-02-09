using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class ShakeManager : MonoBehaviour
    {
        public static ShakeManager Instance;

        private bool _onShake = false;
        private Camera _cam;

        private void Awake()
        {
            #region Singleton
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
            #endregion
            _cam=Camera.main;
        }

        public void ShakeCamera(bool softShake)
        {
            if (_onShake) return;
            
            _onShake = true;
            if (!softShake)
                _cam.DOShakeRotation(.3f, 4, 8, 100, true).OnComplete(EndShake);
            else
                _cam.DOShakeRotation(.1f, 2, 4, 100, true).OnComplete(EndShake);
        }

        private void EndShake()
        {
            _onShake = false;
        }
    }
}