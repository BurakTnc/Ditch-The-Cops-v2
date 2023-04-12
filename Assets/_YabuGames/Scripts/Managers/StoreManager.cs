using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Managers
{
    public class StoreManager : MonoBehaviour
    {
        public static StoreManager Instance;
        
        public int[] mapPrices = new int[4];
        public int[] carPrices = new int[9];

        [HideInInspector] public int[] boughtMaps = new int[4];
        [HideInInspector] public int[] boughtCars = new int[9];

        [SerializeField] private GameObject[] currentMapImages;
        [SerializeField] private Button[] carButtons;
        [SerializeField] private Button[] mapButtons;
        [SerializeField] private TextMeshProUGUI[] mapButtonsTexts;
        [SerializeField] private GameObject[] mapButtonImages;
        [SerializeField] private TextMeshProUGUI[] carButtonTexts;
        [SerializeField] private GameObject[] carButtonImages;

        private int _prevMapId, _prevCarId;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
            GetValues();
        }

        private void Start()
        {
            CheckButtonConditions();
            SetButtons();
            SetCurrentMapImage();
        }

        private void CheckButtonConditions()
        {
            for (var i = 0; i < carButtons.Length; i++)
            {
                if (boughtCars[i] > 0)
                {
                    carButtons[i].interactable = true;
                }
                else
                {
                    carButtons[i].interactable = GameManager.Instance.money >= carPrices[i];
                }
            }
            
            for (var i = 0; i < mapButtons.Length; i++)
            {
                if (boughtMaps[i] > 0)
                {
                    mapButtons[i].interactable = true;
                }
                else
                {
                    mapButtons[i].interactable = GameManager.Instance.money >= mapPrices[i];
                }
            }
        }

        private void SetButtons()
        {
            for (var i = 0; i < boughtMaps.Length; i++)
            {
                if (boughtMaps[i] == 0)
                {
                    var price = mapPrices[i];
                    mapButtonsTexts[i].text = price.ToString();
                }
                else
                {
                    if (_prevMapId==i)
                    {
                        mapButtonsTexts[i].text = "Selected";
                        mapButtonImages[i].SetActive(false);
                        continue;
                    }
                    var anchoredPos = mapButtonsTexts[i].GetComponent<RectTransform>().anchoredPosition;
                    mapButtonsTexts[i].GetComponent<RectTransform>().anchoredPosition =
                        new Vector2(0, anchoredPos.y);
                    mapButtonsTexts[i].text = "Select";
                    mapButtonImages[i].SetActive(false);
                    
                }
                
            }

            for (var i = 0; i < boughtCars.Length; i++)
            {
                if (boughtCars[i] == 0)
                {
                    var price = carPrices[i];
                    carButtonTexts[i].text = price.ToString();
                }
                else
                {
                    if (_prevCarId==i)
                    {
                        carButtonTexts[i].text = "Selected";
                        carButtonImages[i].SetActive(false);
                        continue;
                    }
                    carButtonTexts[i].text = "Select";
                    carButtonImages[i].SetActive(false);
                }
            }
        }

        private void GetValues()
        {
            for (var i = 0; i < boughtMaps.Length; i++)
            {
                if (i==0)
                {
                    boughtMaps[i] = 1;
                }
                else
                {
                    boughtMaps[i] = PlayerPrefs.GetInt($"map{i}", 0);
                }

                _prevMapId = PlayerPrefs.GetInt("prevMapId", 0);

            }

            for (var i = 0; i < boughtCars.Length; i++)
            {
                if (i == 0)
                {
                    boughtCars[i] = 1;
                }
                else
                {
                    boughtCars[i] = PlayerPrefs.GetInt($"car{i}", 0);
                }
                
            }
        }
        private void Save()
        {
            for (var i = 0; i < boughtMaps.Length; i++)
            {
                PlayerPrefs.SetInt($"map{i}", boughtMaps[i]);
            }

            for (var i = 0; i < boughtCars.Length; i++)
            {
                PlayerPrefs.SetInt($"car{i}",boughtCars[i]);
            }
        }

        private void SetCurrentMapImage()
        {
            foreach (var t in currentMapImages)
            {
                t.SetActive(false);
            }

            currentMapImages[_prevMapId].SetActive(true);
        }

        public void UnlockMap(int mapID)
        {
            if (_prevMapId == 0)
            {
                mapButtonsTexts[mapID].text = "Selected";
                mapButtonImages[mapID].SetActive(false);
                _prevMapId = mapID;
                PlayerPrefs.SetInt("prevMapId",_prevMapId);
                SetCurrentMapImage();
            }
            else
            {
                
                mapButtonsTexts[_prevMapId].text = "Select";
                mapButtonsTexts[mapID].text = "Selected";
                mapButtonImages[mapID].SetActive(false);
                _prevMapId = mapID;
                PlayerPrefs.SetInt("prevMapId",_prevMapId);
                SetCurrentMapImage();
        
            }
            currentMapImages[mapID].SetActive(true);
            SceneLoader.Instance.ChangeSceneIndex(mapID+1);
            if (boughtMaps[mapID] == 0)
            {
                GameManager.Instance.money -= mapPrices[mapID];
            }
            boughtMaps[mapID] = 1;
            CheckButtonConditions();
            SetButtons();
            Save();
        }

        public void UnlockCar(int carID)
        {
            if (_prevCarId == 0)
            {
                carButtonTexts[carID].text = "Selected";
                carButtonImages[carID].SetActive(false);
                _prevCarId = carID;
            }
            else
            {
                carButtonTexts[_prevCarId].text = "Select";
                carButtonTexts[carID].text = "Selected";
                carButtonImages[carID].SetActive(false);
                _prevCarId = carID;
        
            }
            if (boughtCars[carID] == 0)
            {
                GameManager.Instance.money -= carPrices[carID];
            }
            boughtCars[carID] = 1;
            PlayerVehicleManager.Instance.SetCarId(carID);
            CheckButtonConditions();
            SetButtons();
            Save();
        }

        
    }
}