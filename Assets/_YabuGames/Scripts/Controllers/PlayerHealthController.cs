using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.ScriptableObjects;
using UnityEngine.UI;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {

        [SerializeField] private CarSpecs specs;

        private float _maxHealth;
        private float _health;

        private void Start()
        {
            _health = specs.health;
            _maxHealth = _health;
        }

        private void SetHealthBar()
        {
            var amount = _health / _maxHealth;
            UIManager.Instance.UpdateHealthBar(amount);
        }
        public void TakeDamage(int damage)
        {
            if (_health<damage)
            {
                _health = 0;
                SetHealthBar();
                //Lose
                return;
            }
            _health -= damage;
            SetHealthBar();
        }
        
    }
}