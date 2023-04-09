using System;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class PlayerVehicleManager : MonoBehaviour
    {
        public static PlayerVehicleManager Instance;
        
       [HideInInspector] public int carID;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }
            Instance = this;

            carID = PlayerPrefs.GetInt("carID", 0);
        }

        private void Start()
        {
            if (transform.childCount <= 0) 
                return;
            
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(carID).gameObject.SetActive(true);
        }

        public void SetCarId(int carID)
        {
            this.carID = carID;
            PlayerPrefs.SetInt("carID",this.carID);
        }
    }
}