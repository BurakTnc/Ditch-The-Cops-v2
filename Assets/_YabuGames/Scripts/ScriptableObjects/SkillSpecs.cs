using UnityEngine;

namespace _YabuGames.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Skill-", menuName = "New Skill", order = 2)]
    public class SkillSpecs : ScriptableObject
    {
        public string headLine;
        public string skillDescription;
        public Sprite skillIcon;
        public int skillID;
    }
}