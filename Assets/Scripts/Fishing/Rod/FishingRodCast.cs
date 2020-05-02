using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishing.Area;
using Utilities;

namespace Fishing.Rod
{
    public class FishingRodCast : MonoBehaviour
    {
        [SerializeField] float _maxStrength = 10f;
        [SerializeField] float _castStrengthSpeed = 1f;
        private Floater _floater = default;
        private FishingControls _controls;
        private bool _held;
        private bool _casted;
        private float _throwStrength;
        private float _timePressing;
        /// <summary>
        /// Point to cast the line to.
        /// </summary>
        private Vector3 _point;
        private LayerMask _checkMask;

        public bool Casting => _held;

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
            _controls.Rod.Throw.canceled += ctx => 
                {
                    UpdateThrowState(); 
                    _casted = ValidatePoint();
                };
            
            _checkMask = LayerMask.GetMask(FishingArea.FISHING_LAYER);
            _casted = false;
        }

        private void Start() 
        {
            _floater = transform.GetComponentRecursive<Floater>();
        }

        private void UpdateThrowState()
        {
            if (_casted)
            {
                _casted = false;
                _floater.PullBack();
            }

            _held = !_held;
        }

        private void Update()
        {
            if (_held && !_casted)
            {
                // Update point until button is released
                _point = GetThrowPoint();
                _timePressing += Time.deltaTime;
            }
            else
            {
                _timePressing = 0.0f;
            }
        }

        private Vector3 GetThrowPoint()
        {
            RaycastHit hit = default;
            Vector3 origin;
            // Ping pong distance along time
            _throwStrength = Mathf.PingPong(_timePressing * _castStrengthSpeed, _maxStrength);
            origin = transform.position;
            origin += transform.forward * _throwStrength;
            // Check position and get the point
            Physics.Raycast(origin, Vector3.down, out hit, 10);

            return hit.point;
        }

        /// <summary>
        /// Validates the current point and casts the line if possible.
        /// </summary>
        /// <returns> True if the line can be cast, false otherwise</returns>
        private bool ValidatePoint()
        {
            var result = Physics.OverlapSphere(_point, .5f, _checkMask);
            FishingArea area;
            foreach(Collider col in result)
            {
                if (col.TryGetComponent<FishingArea>(out area))
                {
                    _floater.Cast(area, _point);
                    return true;
                }
            }
            _throwStrength = 0;
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            DrawStrength(_maxStrength, 0.5f);
            Gizmos.color = Color.blue;
            DrawStrength(_throwStrength, 0.3f);
            Gizmos.DrawSphere(_point, .2f);

            void DrawStrength(float max, float offset)
            {
                Vector3 origin = transform.position;
                // Place an X offset
                origin += transform.right * offset;
                Gizmos.DrawRay(origin, transform.forward * max);
                origin -= transform.right * offset * 2;
                Gizmos.DrawRay(origin, transform.forward * max);
            }
        }
    }
}
