using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

namespace Movement.Cameras
{
    public class CamerasMaster : MonoBehaviour
    {
        private List<CameraHandler> _cameras;
        private CameraType _activeCamera;
        private void Start()
        {
            _cameras = new List<CameraHandler>();
            _cameras = GetComponentsInChildren<CameraHandler>().ToList();
            
        }
    }
}
