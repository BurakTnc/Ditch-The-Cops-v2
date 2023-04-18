using System;
using System.Collections.Generic;
using System.Linq;
using _YabuGames.Scripts.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

        [SerializeField] private int[] mysteryButtonIds;
        [SerializeField] private Button[] carButtons;
        [SerializeField] private Button[] mapButtons;
        [SerializeField] private TextMeshProUGUI[] mapButtonsTexts;
        [SerializeField] private GameObject[] mapButtonImages;
        [SerializeField] private TextMeshProUGUI[] carButtonTexts;
        [SerializeField] private GameObject[] carButtonImages;

        [Header("Watch Buttons")] 
        [SerializeField] private Button[] watchMapButtons = new Button[3];
        [SerializeField] private Button[] watchCarButtons = new Button[8];
        [SerializeField] private TextMeshProUGUI[] watchMapButtonTexts = new TextMeshProUGUI[3];
        [SerializeField] private TextMeshProUGUI[] watchCarButtonTexts = new TextMeshProUGUI[8];
        [SerializeField] private int[] targetWatchMapCounts = new int[3];
        [SerializeField] private int[] targetWatchCarCounts = new int[8];

        private int _prevMapId, _prevCarId;
        private readonly int[] _boughtMysteryCars = new int[9];
        private readonly int[] _watchMapStatus = new int[3];
        private readonly int[] _watchCarStatus = new int[8];

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
            AdSignals.Instance.OnRewardedMapWatchComplete += MapWatchStatus;
            AdSignals.Instance.OnRewardedCarWatchComplete += CarWatchStatus;
        }

        private void UnSubscribe()
        {
            AdSignals.Instance.OnRewardedMapWatchComplete -= MapWatchStatus;
            AdSignals.Instance.OnRewardedCarWatchComplete -= CarWatchStatus;
        }

        private void Start()
        {
            CheckButtonConditions();
            SetButtons();
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
                        
                        //Mystery Icon Close
                        if (mysteryButtonIds.Contains(i) && _boughtMysteryCars[i] > 0)
                        {
                            var buttonParent = carButtons[i].transform.parent.parent;
                            buttonParent.GetChild(buttonParent.childCount-1).gameObject.SetActive(false);
                        }
                        
                        continue;
                    }
                    var anchoredPos = carButtonTexts[i].GetComponent<RectTransform>().anchoredPosition;
                    carButtonTexts[i].GetComponent<RectTransform>().anchoredPosition =
                        new Vector2(0, anchoredPos.y);
                    carButtonTexts[i].text = "Select";
                    carButtonImages[i].SetActive(false);
                    //Mystery Icon Close
                    if (mysteryButtonIds.Contains(i) && _boughtMysteryCars[i] > 0)
                    {
                        var buttonParent = carButtons[i].transform.parent.parent;
                        buttonParent.GetChild(buttonParent.childCount-1).gameObject.SetActive(false);
                    }
                }
            }

            for (var i = 0; i < watchMapButtons.Length; i++)
            {
                var btn = watchMapButtons[i];
                var text = watchMapButtonTexts[i];
                var display=_watchMapStatus[i] + "/" + targetWatchMapCounts[i];
                
                if (_watchMapStatus[i] == -1)
                {
                    btn.gameObject.SetActive(false);
                }
                else
                {
                    text.text = display;
                }
            }
        }

        private void GetValues()
        {
            #region Buttons

            #region MapButtons

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
            }

            #endregion

            #region CarButtons

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

#endregion

            #endregion

            #region MysteryButtons

            for (var i = 0; i < _boughtMysteryCars.Length; i++)
            {
                var id = PlayerPrefs.GetInt($"mysteryCar{i}", 0);
                if (id >0)
                {
                    _boughtMysteryCars[i] = 1;
                }
        
            }

            #endregion

            #region WatchButtons

            #region WatchMapButtons

            for (var i = 0; i < _watchMapStatus.Length; i++)
            {
                var watchStatus = PlayerPrefs.GetInt($"watchMapStatus{i}", 0);
                _watchMapStatus[i] = watchStatus;

            }
            

            #endregion

            #region WatchCarButtons

            

                
            #endregion

            #endregion
            
            _prevMapId = PlayerPrefs.GetInt("prevMapId", 0);
            _prevCarId = PlayerPrefs.GetInt("prevCarId", 0);
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
            PlayerPrefs.SetInt("prevMaoId",_prevMapId);
            PlayerPrefs.SetInt("prevCarId",_prevCarId);
        }

        private void MapWatchStatus(int mapID)
        {
            var id = mapID - 1;
            var status = _watchMapStatus[id];

            status++;
            if (status >= targetWatchMapCounts[id])
            {
                PlayerPrefs.SetInt($"watchMapStatus{id}",-1);
                watchMapButtons[id].gameObject.SetActive(false);
                UnlockMap(mapID);
                return;
            }

            _watchMapStatus[id] = status;
            PlayerPrefs.SetInt($"watchMapStatus{id}",status);
            CheckButtonConditions();
            SetButtons();
            Save();
        }
        private void CarWatchStatus(int mapID)
        {
            var status = _watchMapStatus[mapID];

            status++;
            if (status >= targetWatchMapCounts[mapID])
            {
                PlayerPrefs.SetInt($"watchMapStatus{mapID}", -1);
                watchMapButtons[mapID].gameObject.SetActive(false);
                UnlockMap(mapID);
                return;
            }
            
            PlayerPrefs.SetInt($"watchMapStatus{mapID}",status);
            CheckButtonConditions();
            SetButtons();
            Save();
        }
        
        public void WatchForMapButton(int mapID)
        {
            AdManager.Instance.ShowRewardedMap(mapID);
        }

        public void WatchForCarButton(int carID)
        {
            AdManager.Instance.ShowRewardedCar(carID);
        }
        
        public void UnlockMap(int mapID)
        {
            if (_prevMapId == 0)
            {
                mapButtonsTexts[mapID].text = "Selected";
                mapButtonImages[mapID].SetActive(false);
                _prevMapId = mapID;
                PlayerPrefs.SetInt("prevMapId",_prevMapId);
            }
            else
            {
                
                mapButtonsTexts[_prevMapId].text = "Select";
                mapButtonsTexts[mapID].text = "Selected";
                mapButtonImages[mapID].SetActive(false);
                _prevMapId = mapID;
                PlayerPrefs.SetInt("prevMapId",_prevMapId);

            }
            SceneLoader.Instance.ChangeSceneIndex(mapID);
            if (boughtMaps[mapID] == 0)
            {
                GameManager.Instance.money -= mapPrices[mapID];
            }
            boughtMaps[mapID] = 1;
            CheckButtonConditions();
            SetButtons();
            Save();
            CoreGameSignals.Instance.OnSave?.Invoke();
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
            if (mysteryButtonIds.Contains(carID))
            {
                PlayerPrefs.SetInt($"mysteryCar{carID}",1);
                _boughtMysteryCars[carID] = 1;
            }
            CheckButtonConditions();
            SetButtons();
            Save();
            CoreGameSignals.Instance.OnSave?.Invoke();
        }

        
    }
}