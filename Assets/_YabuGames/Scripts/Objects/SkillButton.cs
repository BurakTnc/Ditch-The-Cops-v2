using System;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _YabuGames.Scripts.Objects
{
    public class SkillButton: MonoBehaviour
    {

        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillDescription, headLine;
        [SerializeField] private GameObject[] crowns = new GameObject[3];

        private int _skillLevel = 1;
        private int _skillID;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        private void SetButtonInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }
        

        public void SetSkill(SkillSpecs skillSpecs)
        {
            headLine.text = skillSpecs.headLine;
            skillDescription.text = skillSpecs.skillDescription;
            skillIcon.sprite = skillSpecs.skillIcon;
            _skillID = skillSpecs.skillID;
            _skillLevel = SkillManager.Instance.ChosenSkills[_skillID];
            foreach (var t in crowns)
            {
                t.SetActive(false);
            }

            if (_skillLevel > 2)
                _skillLevel = 2;
            for (var i = 0; i < _skillLevel + 1; i++)
            {
                crowns[i].SetActive(true);
            }
        }
        

        private void OnEnable()
        {
            SkillSignals.Instance.OnSkillPanelOpened += SetButtonInteractable;
            OnOpening();
        }

        private void OnDisable()
        {
            //SkillSignals.Instance.OnSkillPanelOpened -= SetButtonInteractable;
            foreach (var t in crowns)
            {
                t.transform.DOComplete();
            }
        }

        private void OnOpening()
        {
            foreach (var t in crowns)
            {
                t.transform.DORewind();
                t.transform.DOKill();
            }
            if(_skillLevel>2)
                return;

            crowns[_skillLevel].transform.DOScale(Vector3.one * 1.3f, .3f).SetLoops(-1, LoopType.Yoyo);
        }

        public void ApplySkill()
        {
            SkillSignals.Instance.OnSkillPanelOpened?.Invoke(false);
            HapticManager.Instance.PlaySelectionHaptic();
            UIManager.Instance.CloseSkillPanel();
            LevelSignals.Instance.OnSkillActive?.Invoke(_skillID);
        }
    }
}