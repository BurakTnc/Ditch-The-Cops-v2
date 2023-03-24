using System;
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
            // if (collision.gameObject.CompareTag("Player"))
            // {
            //     _carController.Eliminate(collision.contacts[0].point);
            // }

            if (collision.gameObject.TryGetComponent(out PoliceCarController carController))
            {
                carController.Eliminate(collision.contacts[0].point);
            }
        }
    }
}