using System.Collections;
using _YabuGames.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class CoinScript : MonoBehaviour
    {
        private RectTransform _target;
        private RectTransform _rectTransform;
        private AudioSource _source;
        private int _earnValue = 5;
        private bool _onUI;

        private void Awake()
        {
            _target = GameObject.Find("CoinImage").GetComponent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();
            _source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _rectTransform.pivot = _target.pivot;
            if (_onUI)
            {
                transform.localScale = Vector3.one * 1.5f;
                var x = Random.Range(-200, 100);
                var y = Random.Range(-300, 0);
                _rectTransform.anchoredPosition = new Vector3(x, y, 0);
            }
            StartCoroutine(Latency());
        }

        private IEnumerator Latency()
        {
            var r = Random.Range(.2f, .5f);
            yield return new WaitForSecondsRealtime(r);
            MoveToTarget(Random.Range(.5f, 1f));

        }

        private void MoveToTarget(float time)
        {
            _rectTransform.DOAnchorPos(_target.anchoredPosition, time).SetEase(Ease.InBack)
                .SetUpdate(UpdateType.Late, true).timeScale = 1;
            _rectTransform.DOScale(Vector2.one, time).OnComplete(SetMoney).SetUpdate(UpdateType.Late, true);
                
        }

        private void SetMoney()
        {
            HapticManager.Instance.PlaySelectionHaptic();
            _source.Play();
            GameManager.Instance.ArrangeMoney(_earnValue);
            Destroy(gameObject,1);
        }

        public void SetEarnValue(int value)
        {
            _earnValue = value;
        }

        public void SetScreen(bool onUI)
        {
            _onUI = onUI;
        }
    }
}