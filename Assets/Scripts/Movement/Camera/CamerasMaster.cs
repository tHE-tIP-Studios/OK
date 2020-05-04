using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

namespace Movement.Cameras
{
    public class CamerasMaster : MonoBehaviour
    {
        private List<CameraHandler> _cameras;
        private CameraHandler _activeCamera;
        public CameraType ActiveType => _activeCamera.Type;
        
        private void Awake() 
        {
            CameraSwitch switcher = Resources.Load<CameraSwitch>("CameraSwitch");
            switcher.Master = this;
        }

        private void Start()
        {
            _cameras = new List<CameraHandler>();
            _cameras = GetComponentsInChildren<CameraHandler>().ToList();
            SetNewCamera(CameraType.WALKING);
        }

        public void SetNewCamera(CameraType type, Transform target = null)
        {
            for(int i = 0; i < _cameras.Count; i++)
            {
                if (!_cameras[i].Type.Equals(type))
                {
                    _cameras[i].SetActive(false);
                }
                else
                {
                    _cameras[i].SetActive(true);

                    _activeCamera = _cameras[i];
                }
            }

            if (target != null) _activeCamera.SetNewTarget(target);
        }
    }
}
