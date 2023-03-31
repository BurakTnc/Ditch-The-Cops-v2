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

        private int _skillLevel;
        private int _skillID;

        public void SetSkill(SkillSpecs skillSpecs)
        {
            headLine.text = skillSpecs.headLine;
            skillDescription.text = skillSpecs.skillDescription;
            skillIcon.sprite = skillSpecs.skillIcon;
            _skillID = skillSpecs.skillID;
            for (var i = 0; i < _skillLevel + 1; i++)
            {
                crowns[i].SetActive(true);
            }
        }

        private void OnEnable()
        {
            OnOpening();
        }

        private void OnDisable()
        {
            foreach (var t in crowns)
            {
                t.transform.DOComplete();
            }
        }

        private void OnOpening()
        {
            crowns[_skillLevel].transform.DOScale(Vector3.one * 1.3f, .3f).SetLoops(-1, LoopType.Yoyo);
        }

        public void ApplySkill()
        {
            UIManager.Instance.CloseSkillPanel();
            LevelSignals.Instance.OnSkillActive?.Invoke(_skillID);
        }
    }
}