using System;
using _YabuGames.Scripts.Signals;
using Unity.VisualScripting;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {
        [SerializeField] private PlayerHealthController healthController;
        [SerializeField] private PlayerPhysicsController physicsController;

        private bool _onGodMode;

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
           SkillSignals.Instance.OnGodMode += ApplyGodMode;
       }

       private void UnSubscribe()
       {
           SkillSignals.Instance.OnGodMode -= ApplyGodMode;
       }

       #endregion

       private void ApplyGodMode()
       {
           
       }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PoliceCarController component))
            {
                if (!_onGodMode)
                {
                    physicsController.PoliceCollision(collision.contacts[0].point);
                    healthController.TakeDamage(component.damage);
                }
                else
                {
                    component.Eliminate(collision.contacts[0].point);
                }
                return;
            }

            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                physicsController.ObstacleCollision(collision.contacts[0].point, rb);
                return;
            }
            
        }

      
    }
}
