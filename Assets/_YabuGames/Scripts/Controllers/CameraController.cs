using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 3f;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float clampPositionXMin, clampPositionXMax;
        [SerializeField] private float clampPositionZMin, clampPositionZMax;
        
        private Transform _player;

        private void Awake()
        {
            _player = GameObject.Find("Player").transform;
        }

        private void Update()
        {
            Follow();
        }
        
        private void Follow()
        {
            // if (!_isGameRunning) 
            //     return;
            Vector3 desiredPos = new Vector3(_player.position.x, transform.position.y, _player.position.z) + offset;
             transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
            // transform.position = new Vector3(Mathf.Clamp(transform.position.x, clampPositionXMin, clampPositionXMax),
            //     transform.position.y, Mathf.Clamp(transform.position.z, clampPositionZMin, clampPositionZMax));
        }
    }
}
