using UnityEngine;
using Cinemachine;

namespace Movement
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private CameraType _type;
        public CameraType Type => _type;
        private CinemachineVirtualCamera _camera;

        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetNewTarget(Transform target)
        {
            _camera.Follow = target;
        }
    }
}