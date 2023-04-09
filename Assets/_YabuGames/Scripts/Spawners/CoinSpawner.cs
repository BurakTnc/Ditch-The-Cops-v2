using System.Collections;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Spawners
{
    public class CoinSpawner : MonoBehaviour
    {
        public static CoinSpawner Instance;
        
        private GameObject _target;
        private bool _isWon;
        private Camera _cam;
        private Transform _player;
        
        private void Awake()
        {
            #region Singleton
            if (Instance!=null && Instance!=this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            #endregion
            _cam=Camera.main;
            _player = GameObject.Find("Player").transform;
            _target=GameObject.Find("CoinImage");
        }

        private void Start()
        {
            CoreGameSignals.Instance.OnSpawnCoins += SpawnCoins;
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnSpawnCoins -= SpawnCoins;
        }

        private IEnumerator Begin(int coin,float delay,int earnValue,bool onUI)
        {
            yield return new WaitForSeconds(delay);
            if (coin <= 0) yield break;

            for (var i = 0; i < coin; i++)
            {
                if (onUI)
                {
                    _target = GameObject.FindGameObjectWithTag("CoinUI");
                }
                var temp = Instantiate(Resources.Load<GameObject>(path: "Spawnables/Coin"),
                    _cam.WorldToScreenPoint(_player.position),
                    _target.transform.rotation, _target.transform.parent);
                temp.GetComponent<CoinScript>().SetScreen(onUI);
                temp.GetComponent<CoinScript>().SetEarnValue(earnValue);
            }
        }

        private void SpawnCoins(int coin,float delay,int earnValue,bool onUI)
        {
            StartCoroutine(Begin(coin, delay, earnValue,onUI));
        }
    }
}