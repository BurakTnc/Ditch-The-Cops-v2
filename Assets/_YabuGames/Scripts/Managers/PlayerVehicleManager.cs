using System;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class PlayerVehicleManager : MonoBehaviour
    {
        public static PlayerVehicleManager Instance;
        
        private int _carID;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }
            Instance = this;

            _carID = PlayerPrefs.GetInt("carID", 0);
        }

        private void Start()
        {
            if (transform.childCount <= 0) 
                return;
            
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(_carID).gameObject.SetActive(true);
        }

        public void SetCarId(int carID)
        {
            _carID = carID;
            PlayerPrefs.SetInt("carID",_carID);
        }
    }
}