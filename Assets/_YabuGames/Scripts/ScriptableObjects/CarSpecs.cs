using UnityEngine;

namespace _YabuGames.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CarSpec-", menuName = "YabuGames/New Car Spec", order = 0)]
    public class CarSpecs : ScriptableObject
    {
        public float angularSpeed = 100;
        public float forwardSpeed = 10;
        public float skidLimit;
        public float topSpeed;
    }
}