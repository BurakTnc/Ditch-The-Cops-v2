using UnityEngine;

namespace _YabuGames.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Police Spec", menuName = "New Police Spec", order = 1)]
    public class PoliceSpecs : ScriptableObject
    {
        public float angularSpeed;
        public float speed;
        public float acceleration;
        public int damage;
        [Range(0, 1)] public float chaseSkill = .1f;
    }
}