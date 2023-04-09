using System;
using System.Collections;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;
       [HideInInspector] public int sceneID;

       [SerializeField] private GameObject loadingPanel;
       [SerializeField] private Image loadingBar;


       private void Awake()
       {
           if (Instance != this && Instance != null) 
           {
               Destroy(this);
               return;
           }

           Instance = this;
           sceneID = PlayerPrefs.GetInt("sceneID", 1);
       }

       private void Start()
       {
           loadingPanel.SetActive(true);
           loadingPanel.transform.GetChild(0).transform.DOLocalMoveX(-600, .7f).SetEase(Ease.InSine).SetRelative(true);
           loadingPanel.transform.GetChild(1).transform.DOLocalMoveX(700, .7f).SetEase(Ease.InSine).SetRelative(true)
               .OnComplete(ClosePanel);

       }

       private void ClosePanel()
       {
           loadingPanel.SetActive(false);
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
           CoreGameSignals.Instance.OnGameStart += BeginLoading;
           CoreGameSignals.Instance.OnMainMenu += BeginMainMenuLoading;
       }

       private void UnSubscribe()
       {
           CoreGameSignals.Instance.OnGameStart -= BeginLoading;
           CoreGameSignals.Instance.OnMainMenu -= BeginMainMenuLoading;
       }

       #endregion

       private IEnumerator LoadSceneAsync(int sceneId)
       {
           var operation = SceneManager.LoadSceneAsync(sceneId);
           loadingPanel.SetActive(true);
           while (!operation.isDone)
           {
               // var progressValue = Mathf.Clamp01(operation.progress / 0.9f);
               // loadingBar.fillAmount = progressValue;
               yield return null;
           }
           
       }
       private void LoadMainMenu()
       {
           StartCoroutine(LoadSceneAsync(0));
       }

       private void BeginMainMenuLoading()
       {
           loadingPanel.SetActive(true);
           loadingPanel.transform.GetChild(0).transform.DOLocalMoveX(600, 1f).SetEase(Ease.InSine).SetRelative(true);
           loadingPanel.transform.GetChild(1).transform.DOLocalMoveX(-700, 1f).SetEase(Ease.InSine).SetRelative(true)
               .OnComplete(LoadMainMenu);
       }

       private void BeginLoading()
       {
           loadingPanel.SetActive(true);
           loadingPanel.transform.GetChild(0).transform.DOLocalMoveX(600, 1f).SetEase(Ease.InSine).SetRelative(true);
           loadingPanel.transform.GetChild(1).transform.DOLocalMoveX(-700, 1f).SetEase(Ease.InSine).SetRelative(true)
               .OnComplete(LoadScene);
       }
       private void LoadScene()
       {
           
           PlayerPrefs.SetInt("sceneID",sceneID);
           StartCoroutine(LoadSceneAsync(sceneID));
       }
       
    }
}