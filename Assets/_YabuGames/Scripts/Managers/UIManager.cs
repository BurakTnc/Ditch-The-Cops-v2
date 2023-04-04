using System;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField] private GameObject mainPanel, gamePanel, winPanel, skillPanel, losePanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;
        [SerializeField] private Image[] stars = new Image[5];
        [SerializeField] private Image healthBar;
        [SerializeField] private Sprite yellowStarSprite;




        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null)
            {

                Destroy(this);
                return;
            }
            
            Instance = this;

            #endregion
            

        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Start()
        {
            SetMoneyTexts();
            
        }

        #region Subscribtions
        private void Subscribe()
                {
                    CoreGameSignals.Instance.OnLevelWin += LevelWin;
                    CoreGameSignals.Instance.OnLevelFail += LevelLose;
                    //CoreGameSignals.Instance.OnGameStart += OnGameStart;
                }
        
                private void UnSubscribe()
                {
                    CoreGameSignals.Instance.OnLevelWin -= LevelWin;
                    CoreGameSignals.Instance.OnLevelFail -= LevelLose;
                    //CoreGameSignals.Instance.OnGameStart -= OnGameStart;
                }

        #endregion
        
        private void OnGameStart()
        {
            // mainPanel.SetActive(false);
            // gamePanel.SetActive(true);
        }
        private void SetMoneyTexts()
        {
            if (moneyText.Length <= 0) return;

            foreach (var t in moneyText)
            {
                if (t)
                {
                    t.text = "$" + GameManager.Instance.GetMoney();
                }
            }
        }
        private void LevelWin()
        {
            Time.timeScale = 0;
            gamePanel.SetActive(false);
            winPanel.transform.localScale = Vector3.zero;
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
            HapticManager.Instance.PlaySuccessHaptic();
        }

        private void LevelLose()
        {
            Time.timeScale = 0;
            gamePanel.SetActive(false);
            losePanel.transform.localScale = Vector3.zero;
            losePanel.SetActive(true);
            losePanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
            HapticManager.Instance.PlayFailureHaptic();
        }
        
        public void UpdateHealthBar(float amount)
        {
            healthBar.fillAmount = amount;
        }

        public void PlayMaxHpAnimation()
        {
            healthBar.transform.parent.DOScaleX(1.1f, .3f).SetLoops(6, LoopType.Yoyo);
        }
        public void SetStars(int wantedLevel)
        {
            for (var i = 0; i < wantedLevel; i++)
            {
                stars[i].sprite = yellowStarSprite;
                stars[i].transform.DOScale(Vector3.one * 1.5f, .3f).SetLoops(10, LoopType.Yoyo);
            }
        }
        public void OpenSkillPanel()
        {
            Time.timeScale = 0;
            skillPanel.transform.localScale=Vector3.zero;
            skillPanel.SetActive(true);
            skillPanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutSine);
        }
        public void CloseSkillPanel()
        {
            skillPanel.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack).OnComplete(Continue);

            void Continue()
            {
                Time.timeScale = 1;
                skillPanel.SetActive(false);
                LevelSignals.Instance.OnSkillPanel?.Invoke(false);
            }
        }

        public void OpenPanel(GameObject panel)
        {
            if(!panel)
                return;
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
        }

        public void SelectMapButton(int sceneID)
        {
            SceneLoader.Instance.sceneID = sceneID;
        }
        public void PlayButton()
        {
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void MenuButton()
        {
            CoreGameSignals.Instance.OnMainMenu?.Invoke();
            HapticManager.Instance.PlayLightHaptic();
        }

        public void NextButton()
        {
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }

        public void RetryButton()
        {
            CoreGameSignals.Instance.OnGameStart?.Invoke();
            HapticManager.Instance.PlaySelectionHaptic();
        }
        
    }
}
