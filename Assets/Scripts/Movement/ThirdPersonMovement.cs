using UnityEngine;
using Fishing.Rod;

namespace Movement
{
    public class ThirdPersonMovement : MonoBehaviour
    {
        [SerializeField] private float _maxSpeed = 5f;
        [SerializeField] private float _angularSpeed = 10f;
        [SerializeField] private float _crouchModifier = .5f;
        private float _speed;
        private CharacterController _playerController;
        private FishingRodCast _fishingRod;
        private PlayerControls _playerControls;
        private Vector2 _movementDir = default;
        private Vector2 _lookDir = default;
        private bool _crouching = false;

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _playerControls.Movement.Direction.performed += ctx => _movementDir = ctx.ReadValue<Vector2>();
            _playerControls.Movement.Look.performed += ctx => _lookDir = ctx.ReadValue<Vector2>();
            _playerControls.Movement.Direction.canceled += ctx => _movementDir = default;
            _playerControls.Movement.Crouch.performed += ctx => _crouching = ctx.ReadValueAsButton();
            _playerControls.Movement.Crouch.canceled += ctx => _crouching = ctx.ReadValueAsButton();

            _playerController = GetComponent<CharacterController>();
            _fishingRod = GetComponent<FishingRodCast>();

            // Init variables
            _speed = _maxSpeed;
        }

        private void Update()
        {
            CheckForModifier();
            UpdateVelocity();
        }

        private void CheckForModifier()
        {
            if (_crouching)
            {
                ChangeSpeed(_crouchModifier);
            }
            else
            {
                ChangeSpeed(1f);
            }
        }

        private void UpdateVelocity()
        {
            Vector3 movement = default;
            movement.x = _movementDir.y * _speed;
            movement.z = _movementDir.x * _speed;
            
            if (!_fishingRod.Casting)
            {
                _playerController.Move(movement * Time.deltaTime);
            }

            if (movement.magnitude > 0.2f)
            {
                UpdateRotation(movement * Time.deltaTime);
                _lookDir = _movementDir;
            }
            //! else if (_lookDir.magnitude > 0.2f)
            //! {
            //!     Vector3 lookVec = new Vector3(_lookDir.y, 0, _lookDir.x);
            //!     UpdateRotation(lookVec * Time.deltaTime);
            //! }
        }

        private void UpdateRotation(Vector3 moveDir)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * _angularSpeed);
        }

        public void ChangeSpeed(float modifier)
        {
            _speed = _maxSpeed * modifier;
        }
    }
}

