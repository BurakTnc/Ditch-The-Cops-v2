using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;
        
    [Header("                               // Set Particles Stop Action To DISABLE //")]
    [Space(20)]
        [SerializeField] private List<GameObject> hitParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> eliminatedParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> explosionParticle = new List<GameObject>();
        [SerializeField] private List<GameObject> smokeParticle = new List<GameObject>();

        
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
            
        }

        public void GetHitParticle(Vector3 desiredPos)
        {
            var temp = hitParticle[0];
            hitParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            hitParticle.Add(temp);
            
        }
        public void GetEliminatedParticle(Vector3 desiredPos)
        {
            var temp = eliminatedParticle[0];
            eliminatedParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            eliminatedParticle.Add(temp);
        }
        public void GetExplosionParticle(Vector3 desiredPos)
        {
            var temp = explosionParticle[0];
            explosionParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            explosionParticle.Add(temp);
        }
        public void GetSmokeParticle(Vector3 desiredPos)
        {
            var temp = smokeParticle[0];
            smokeParticle.Remove(temp);
            temp.transform.position = desiredPos;
            temp.SetActive(true);
            smokeParticle.Add(temp);
        }
    }
}
