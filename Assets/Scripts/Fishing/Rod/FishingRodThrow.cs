using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing.Rod
{
    public class FishingRodThrow : MonoBehaviour
    {
        [SerializeField] float _maxStrength;
        private FishingControls _controls;
        private bool _held;
        private float _throwStrength;
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
            _controls.Rod.Throw.canceled += ctx => UpdateThrowState();
            _checkMask = LayerMask.GetMask();
        }

        private void UpdateThrowState()
        {
            _held = !_held;
        }

        private void Update()
        {
            Vector3 point = default;
            if (_held)
            {
                point = GetThrowPoint();
                // Update travel distance here
                
            }
            else
            {
                _throwStrength = 0.0f;
                if (ValidThrowPoint(point))
                {

                }
            }
        }

        private Vector3 GetThrowPoint()
        {
            RaycastHit hit = default;
            Vector3 origin;
            _throwStrength = Mathf.PingPong(_throwStrength, _maxStrength);
            origin = transform.position;
            origin.z += _throwStrength;
            if (Physics.Raycast(origin, Vector3.down, 10, _checkMask))
            {

            }

            return hit.point;
        }

        private bool ValidThrowPoint(Vector3 point)
        {
            return true;
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
