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
                carController.Eliminate(collision.contacts[0].point);
                return;
            }

            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                if (collision.gameObject.CompareTag("Player"))
                    return;
                PoolManager.Instance.GetHitParticle(collision.contacts[0].point);
                rb.constraints = RigidbodyConstraints.None;
                Destroy(collision.gameObject,3);
            }
            
        }
    }
}