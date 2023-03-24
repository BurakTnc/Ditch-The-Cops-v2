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
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log(collision.transform.position);
                _carController.Eliminate(collision.transform.position+Vector3.down);
            }
        }
    }
}