using UnityEngine;
using System;
using _YabuGames.Scripts.Signals;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerInputController : MonoBehaviour
    {
       [HideInInspector] public Vector3 direction;

        private float _xAxis, _yAxis;

        
        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InputSignals.Instance.OnTouch?.Invoke(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                InputSignals.Instance.OnTouch?.Invoke(false);
            }
            _xAxis = SimpleInput.GetAxis("Horizontal");
            _yAxis = SimpleInput.GetAxis("Vertical");
            // if (_xAxis>0)
            // {
            //     _xAxis = 1;
            // }
            //
            // if (_xAxis<0)
            // {
            //     _xAxis = -1;
            // }
            //
            // if (_yAxis>0)
            // {
            //     _yAxis = 1;
            // }
            //
            // if (_yAxis<0)
            // {
            //     _yAxis = -1;
            // }
            direction = Vector3.forward * _yAxis + Vector3.right * _xAxis;
        }
        
    }
}
