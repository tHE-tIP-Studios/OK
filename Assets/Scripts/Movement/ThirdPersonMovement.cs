using UnityEngine;

namespace Movement
{
    public class ThirdPersonMovement : MonoBehaviour
    {
        [SerializeField] private float _maxSpeed = 5f;
        [SerializeField] private float _crouchModifier = .5f;
        private float _speed;
        private CharacterController _playerController;
        private PlayerControls _playerControls;
        private Vector2 _movementDir = default;
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
            _playerControls.Movement.Direction.canceled += ctx => _movementDir = default;
            _playerControls.Movement.Crouch.performed += ctx => _crouching = ctx.ReadValueAsButton();
            _playerControls.Movement.Crouch.canceled += ctx => _crouching = ctx.ReadValueAsButton();

            _playerController = GetComponent<CharacterController>();

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
            _playerController.Move(movement * Time.deltaTime);
        }

        public void ChangeSpeed(float modifier)
        {
            _speed = _maxSpeed * modifier;
        }
    }
}

