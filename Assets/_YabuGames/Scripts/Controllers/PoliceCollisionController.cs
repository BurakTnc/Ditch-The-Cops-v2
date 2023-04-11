using System;
using _YabuGames.Scripts.Managers;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PoliceCollisionController : MonoBehaviour
    {
        private PoliceCarController _carController;

        private void Awake()
        {
            _carController = GetComponent<PoliceCarController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            
            if (collision.gameObject.TryGetComponent(out PoliceCarController carController))
            {
                if (carController.isArmored)
                {
                    if(!_carController.isArmored)
                        return;
                }
                    
                carController.Eliminate(collision.contacts[0].point);
                return;
            }

            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    if (!_carController.onOil)
                    {
                        _carController.Stop();
                        return;
                    }
                        
                    _carController.Eliminate(collision.contacts[0].point);
                    return;

                }
                PoolManager.Instance.GetHitParticle(collision.contacts[0].point);
                rb.constraints = RigidbodyConstraints.None;
                Destroy(collision.gameObject,3);
            }
            
            
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!_carController.onOil)
                {
                    _carController.Continue();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("SpikeTrap"))
            {
                if(_carController.isArmored)
                    return;
                _carController.Eliminate(transform.position + Vector3.down);
                return;
            }

            if (other.gameObject.CompareTag("OilTrap"))
            {
                _carController.OnOilTrap();
            }
        }
    }
}