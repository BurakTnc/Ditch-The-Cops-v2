using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {
        [SerializeField] private PlayerHealthController healthController;
        
       [SerializeField] private PlayerPhysicsController _physicsController;

        private void Awake()
        {
            //_physicsController = GetComponent<PlayerPhysicsController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PoliceCarController component))
            {
                _physicsController.PoliceCollision(collision.contacts[0].point);
                healthController.TakeDamage(component.damage);
            }
            
        }

      
    }
}
