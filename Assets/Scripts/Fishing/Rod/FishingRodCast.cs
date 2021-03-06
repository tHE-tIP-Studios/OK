﻿using Fishing.Area;
using Movement.Cameras;
using UnityEngine;

namespace Fishing.Rod
{
    public class FishingRodCast : MonoBehaviour
    {
        [SerializeField] float _maxStrength = 16f;
        [SerializeField] float _minStrength = 4f;
        [SerializeField] float _castStrengthSpeed = 1f;
        [SerializeField] Floater _floaterPrefab;
        [SerializeField] LineHandler _floaterLine;
        [SerializeField] CameraSwitch _switch;
        private Floater _currentFloater = default;
        private FishingControls _controls;
        private bool _held;
        private float _throwStrength;
        private float _timePressing;
        /// <summary>
        /// Point to cast the line to.
        /// </summary>
        private Vector3 _point;
        private LayerMask _checkMask;

        public bool Casting => _held || Casted;
        public bool Casted { get; set; }

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
            _controls.Rod.Throw.performed += ctx => OnButtonPress();
            _controls.Rod.Throw.canceled += ctx =>
            {
                OnButtonRelease();
                Debug.Log(Casted);
                if (!Casted)
                {
                    Casted = ValidatePoint();
                }
                else
                {
                    Casted = !Casted;
                }
            };

            _checkMask = LayerMask.GetMask(FishingArea.FISHING_LAYER);
            Casted = false;
        }
        private void OnButtonPress()
        {
            if (Casted)
            {
                PullBack();
            }
            _held = true;
        }

        private void PullBack()
        {
            //_floaterLine.Release();
            _switch.TurnOnWalkCamera();
            _currentFloater.ReleaseFish();
            if (_currentFloater != null)
            {
                //_floaterLine.PullLine(5);
                LeanTween.move(_currentFloater.gameObject, _floaterLine.transform, 0.2f).
                setOnStart(OnStart).
                setOnComplete(x=> 
                {
                    GameObject.Destroy(_currentFloater.gameObject);
         
                    _currentFloater = Instantiate(_floaterPrefab, _floaterLine.transform.position,
                        _floaterLine.transform.rotation);
                    _floaterLine.NewTarget(_currentFloater.transform);
                });
            }

            void OnStart()
            {
                _floaterLine.NewLine(_currentFloater.transform, 0.9f, true);
            }
        }

        private void OnFail()
        {
            PullBack();
            Casted = false;
        }

        private void OnButtonRelease()
        {
            _held = false;
        }

        private void Update()
        {
            if (_held && !Casted)
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
            _throwStrength = Mathf.Clamp(_throwStrength, _minStrength, _maxStrength);
            origin = transform.position;
            origin += transform.forward * _throwStrength;
            // Check position and get the point
            Physics.Raycast(origin, Vector3.down, out hit, 10);

            return hit.point;
        }

        /// <summary>
        /// Validates the current point and casts the line if possible.
        /// </summary>
        /// <returns> True if the line was cast, false otherwise</returns>
        private bool ValidatePoint()
        {
            var result = Physics.OverlapSphere(_point, .5f, _checkMask);
            FishingArea area;
            foreach (Collider col in result)
            {
                if (col.TryGetComponent<FishingArea>(out area))
                {
                    if (_currentFloater) Destroy(_currentFloater.gameObject);
                    // Create and throw a new floater.
                    _currentFloater = Instantiate(_floaterPrefab, _floaterLine.transform.position,
                        _floaterLine.transform.rotation);
                    Debug.Log(_point);
                    _currentFloater.Cast(area, _point, OnFail);
                    _floaterLine.NewLine(_currentFloater.transform, 0.9f);
                    _switch.TurnOnFishingCamera(_currentFloater.transform);
                    Casted = true;
                    return true;
                }
            }
            _throwStrength = 0;
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            DrawStrength(_maxStrength, 0.5f);
            Gizmos.color = Color.red;
            DrawStrength(_minStrength, 0.5f);
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