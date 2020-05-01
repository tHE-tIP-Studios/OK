using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement.Cameras
{
    public class CameraSwitch : MonoBehaviour
    {
        private CamerasMaster _master;

        private void Awake() 
        {
            _master = GetComponent<CamerasMaster>();
        }

        public void TurnOnWalk()
        {
            _master.SetNewCamera(CameraType.WALKING);
        }

        public void TurnOnCrouch()
        {
            _master.SetNewCamera(CameraType.CROUCH);
        }

        public void TurnOnFish()
        {
            _master.SetNewCamera(CameraType.FISHING);
        }
    }
}
