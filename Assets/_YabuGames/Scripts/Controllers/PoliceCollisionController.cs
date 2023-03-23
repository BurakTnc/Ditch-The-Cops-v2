using System;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PoliceCollisionController : MonoBehaviour
    {
        private PoliceCarController _carController;
        private PoliceAIController _aiController;

        private void Awake()
        {
            _carController = GetComponent<PoliceCarController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _carController.Eliminate(collision.transform.position+Vector3.down);
            }
        }
    }
}