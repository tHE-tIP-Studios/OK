using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishing.Area;

namespace Fishing.Rod
{
    public class FishingRodCast : MonoBehaviour
    {
        [SerializeField] float _maxStrength;
        private FishingControls _controls;
        private bool _held;
        private float _throwStrength;
        /// <summary>
        /// Point to cast the line to.
        /// </summary>
        private Vector3 _point;
        private LayerMask _checkMask;

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Awake()
        {
            _controls = new FishingControls();
            _controls.Rod.Throw.performed += ctx => UpdateThrowState();
            _controls.Rod.Throw.canceled += ctx => {UpdateThrowState(); ValidatePoint();};
            _checkMask = LayerMask.GetMask(FishingArea.FISHING_LAYER);
        }

        private void UpdateThrowState()
        {
            _held = !_held;
        }

        private void Update()
        {
            if (_held)
            {
                _point = GetThrowPoint();
                // Update travel distance here
            }
        }

        private Vector3 GetThrowPoint()
        {
            RaycastHit hit = default;
            Vector3 origin;
            _throwStrength = Mathf.PingPong(_throwStrength, _maxStrength);
            origin = transform.position;
            origin.z += _throwStrength;
            Physics.Raycast(origin, Vector3.down, out hit, 10);

            return hit.point;
        }

        private bool ValidatePoint()
        {
            return Physics.CheckSphere(_point, .5f, _checkMask);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            DrawStrength(_maxStrength, 0.5f);
            Gizmos.color = Color.blue;
            DrawStrength(_throwStrength, 0.3f);

            void DrawStrength(float max, float offset)
            {
                Vector3 destination = transform.position;
                Vector3 origin = transform.position;
                destination.z += max;
                // Place an X offset
                destination.x += offset;
                origin.x += offset;
                Gizmos.DrawLine(origin, destination);
                origin.x -= offset * 2;
                destination.x -= offset * 2;
                Gizmos.DrawLine(origin, destination);
                Gizmos.color = Color.blue;
            }
        }
    }
}
