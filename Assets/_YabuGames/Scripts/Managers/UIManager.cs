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

        [Header("Player Profile")] 
        [SerializeField] private TextMeshProUGUI playerXpText;
        [SerializeField] private TextMeshProUGUI playerLevel;
        [SerializeField] private Image playerProgressBar;
        [Header("Missions")] 
        [SerializeField] private TextMeshProUGUI eliminatedCopsText;
        [SerializeField] private TextMeshProUGUI survivedTimeText;
        [SerializeField] private TextMeshProUGUI reachedLevelText;
        [SerializeField] private Slider eliminatedCopsProgressBar;
        [SerializeField] private Slider survivedTimeProgressBar;
        [SerializeField] private Slider reachedLevelProgressBar;
        [SerializeField] private Button[] missionClaimButtons;
        



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
            if(!playerXpText)
                return;
            var reachedXp = GameManager.Instance.GetPlayerXp();
            
            var reachedXpAmount = reachedXp / 1000;
            playerProgressBar.DOFillAmount(reachedXpAmount, 1).SetEase(Ease.OutBack);
            playerXpText.text = reachedXp + "/1000";
            
        }

        private void SetProgressBars()
        {
            if (!eliminatedCopsProgressBar)
                return;

            var eliminatedCops = GameManager.Instance.GetEliminatedCops();
            
            eliminatedCopsProgressBar.DOValue(eliminatedCops, 1).SetEase(Ease.OutBack);
            eliminatedCopsText.text = eliminatedCops + "/100";

            var survivedTime = GameManager.Instance.GetSurvivedTime();
            
            survivedTimeProgressBar.DOValue(survivedTime/60, 1).SetEase(Ease.OutBack);
            survivedTimeText.text = (int) (survivedTime / 60) + "/100";


            var pLevel = GameManager.Instance.GetPlayerLevel();
            reachedLevelText.text = pLevel + "/100";
            reachedLevelProgressBar.DOValue(pLevel, 1).SetEase(Ease.OutBack);
            playerLevel.text = pLevel.ToString();
        }

        private void Update()
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
                    t.text = GameManager.Instance.GetMoney().ToString();
                }
            }
        }
        private void LevelWin()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
            Time.timeScale = 0;
            gamePanel.SetActive(false);
            winPanel.transform.localScale = Vector3.zero;
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
            HapticManager.Instance.PlaySuccessHaptic();
        }

        private void LevelLose()
        {
            CoreGameSignals.Instance.OnSave?.Invoke();
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

        public void OpenMissionsPanel( GameObject panel)
        {
            if(!panel)
                return;
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
            SetProgressBars();
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

        public void OpenClaimButton(int buttonID)
        {
            if(missionClaimButtons.Length<1)
                return;
            
            missionClaimButtons[buttonID].interactable = true;
        }

        public void ClaimButton(int buttonID)
        {
            missionClaimButtons[buttonID].interactable = false;
            CoreGameSignals.Instance.OnSpawnCoins?.Invoke(20, 1, 50, true);
        }
    }
}
