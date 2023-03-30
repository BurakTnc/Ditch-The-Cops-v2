using System;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class RendererChecker : MonoBehaviour
    {
        private void OnBecameInvisible()
        {
            Debug.Log("NO");
        }

        private void OnBecameVisible()
        {
            Debug.Log("YES");
        }
    }
}