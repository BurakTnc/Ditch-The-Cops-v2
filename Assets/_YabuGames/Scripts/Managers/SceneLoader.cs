using System;
using _YabuGames.Scripts.Signals;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _YabuGames.Scripts.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;
       [HideInInspector] public int sceneID;


       private void Awake()
       {
           if (Instance != this && Instance != null) 
           {
               Destroy(this);
               return;
           }

           Instance = this;
       }

       private void Start()
       {
           sceneID = PlayerPrefs.GetInt("sceneID", 0);
       }

       #region Subscribtions

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
           CoreGameSignals.Instance.OnGameStart += LoadScene;
           CoreGameSignals.Instance.OnMainMenu += LoadMainMenu;
       }

       private void UnSubscribe()
       {
           CoreGameSignals.Instance.OnGameStart -= LoadScene;
           CoreGameSignals.Instance.OnMainMenu -= LoadMainMenu;
       }

       #endregion

       private void LoadMainMenu()
       {
           SceneManager.LoadScene(1);
       }
       public void LoadScene()
       {
           PlayerPrefs.SetInt("sceneID",sceneID);
           SceneManager.LoadScene(sceneID);
       }
       
    }
}