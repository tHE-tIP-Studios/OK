using UnityEngine;
using Cinemachine;

namespace Movement
{
    public class CameraHandler : MonoBehaviour
    {
        private const int MAX_PRIORITY = 10;
        [SerializeField] private CameraType _type;
        public CameraType Type => _type;
        private CinemachineVirtualCamera _camera;

        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetActive(bool value)
        {
            _camera.Priority = value ? MAX_PRIORITY : 0;
        }

        public void SetNewTarget(Transform target)
        {
            _camera.Follow = target;
        }

        public void SetPositionXZ(float x, float z)
        {
            Vector3 position = transform.position;
            position.x = x;
            position.z = z;
            transform.position = position;
        }
    }
}