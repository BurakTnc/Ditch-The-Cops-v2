using System;
using System.Collections;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using _YabuGames.Scripts.Spawners;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {
        [SerializeField] private PlayerHealthController healthController;
        [SerializeField] private PlayerPhysicsController physicsController;
        [SerializeField] private AudioClip hitSound;

        private bool _onGodMode;
        private bool _onReduceDamage;
        private bool _isEliminated;

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
           SkillSignals.Instance.OnReduceDamage += ApplyReducedDamage;
           LevelSignals.Instance.OnPlayerDestroyed += Eliminate;
           LevelSignals.Instance.OnRevive += Revive;
       }

       private void UnSubscribe()
       {
           SkillSignals.Instance.OnGodMode -= ApplyGodMode;
           SkillSignals.Instance.OnReduceDamage -= ApplyReducedDamage;
           LevelSignals.Instance.OnPlayerDestroyed -= Eliminate;
           LevelSignals.Instance.OnRevive -= Revive;
       }

       #endregion

       private void Eliminate()
       {
           _isEliminated = true;
       }

       private void Revive()
       {
           _isEliminated = false;
       }

       private void ApplyReducedDamage()
       {
           _onReduceDamage = true;
       }
       private void ApplyGodMode(float duration)
       {
           StartCoroutine(Delayer(duration));
       }

       private IEnumerator Delayer(float duration)
       {
           _onGodMode = true;
           yield return new WaitForSeconds(duration);
           _onGodMode = false;
       }
        private void OnCollisionEnter(Collision collision)
        {
            if(_isEliminated)
                return;
            if (collision.gameObject.TryGetComponent(out PoliceCarController component))
            {
                if (!_onGodMode)
                {
                    HapticManager.Instance.PlayHeavyHaptic();
                    physicsController.PoliceCollision(collision.contacts[0].point);
                    if (_onReduceDamage)
                    {
                        var reducedDamage = (int)(component.damage - component.damage * .1f);
                        healthController.TakeDamage(reducedDamage);
                    }
                    else
                    {
                        healthController.TakeDamage(component.damage);
                    }
                    
                }
                else
                {
                    HapticManager.Instance.PlayRigidHaptic();
                    component.MissileExplosion(collision.contacts[0].point,10);
                }
                return;
            }

            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                HapticManager.Instance.PlaySelectionHaptic();
                physicsController.ObstacleCollision(collision.contacts[0].point, rb);
                AudioSource.PlayClipAtPoint(hitSound,transform.position);
                return;
            }
            else
            {
                physicsController.PoliceCollision(collision.contacts[0].point);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                if (other.transform.parent.TryGetComponent(out Spawner spawner))
                {
                    spawner.hasCollectible = false;
                }
                SpawnManager.Instance.currentCollectibleCount--;
                SpawnManager.Instance.collectibleDelayer += 2;
                var r = Random.Range(SkillManager.Instance.earningLevel, 6);
                CoreGameSignals.Instance.OnSpawnCoins?.Invoke(r, 0, 10,false);
                Destroy(other.gameObject);
            }
            if (other.gameObject.CompareTag("Health"))
            {
                if (other.transform.parent.TryGetComponent(out Spawner spawner))
                {
                    spawner.hasCollectible = false;
                }
                SpawnManager.Instance.currentCollectibleCount--;
                SpawnManager.Instance.collectibleDelayer += 2;
                healthController.GetHeal();
                Destroy(other.gameObject);
            }
        }
    }
}
