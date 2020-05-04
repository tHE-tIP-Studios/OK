using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement.Cameras
{
    [CreateAssetMenu(menuName = "OK/CameraSwitcher")]
    public class CameraSwitch : ScriptableObject
    {
        private CamerasMaster _master;
        public CamerasMaster Master {set{_master = value;}}

        public void TurnOnWalkCamera()
        {
            _master.SetNewCamera(CameraType.WALKING);
        }

        public void TurnOnCrouch()
        {
            _master.SetNewCamera(CameraType.CROUCH);
        }

        public void TurnOnFishingCamera(Transform target)
        {
            _master.SetNewCamera(CameraType.FISHING, target);
        }
    }
}
