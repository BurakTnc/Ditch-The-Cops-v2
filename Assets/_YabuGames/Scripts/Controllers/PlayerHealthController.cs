using _YabuGames.Scripts.ScriptableObjects;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {
        [SerializeField] private CarSpecs specs;
        
        private float _health;

        private void Start()
        {
            _health = specs.health;
        }

        public void TakeDamage(int damage)
        {
            if (_health<damage)
            {
                _health = 0;
                return;
            }
            _health -= damage;
        }
    }
}