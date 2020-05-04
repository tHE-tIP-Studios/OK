using System;
using System.Collections;
using Fauna;
using Fauna.Animals;
using Fishing.Area;
using UnityEngine;

namespace Fishing
{
    public class FishingBehaviour : MonoBehaviour
    {
        private const float GETAWAY_THRESHOLD = 1;
        private FishingControls _fishingControls;
        private PlayerControls _playerControls;
        private bool _reelPressed;

        private int _staminaPerlinSeed;
        private int _steerPerlinSeed;

        private float _fishStamina;
        private float _getawayCount;

        private float _reelInWindowTimer;
        private int _reelsRequired;
        private int _reelCount;

        private float NextWindow => (FishInfo.CatchingValues.Stamina - 1) - _reelCount;

        public float FishStaminaPercentage => _fishStamina / FishInfo.CatchingValues.Stamina;
        public int TendencySide => _fishTendencySide;
        public int Fails { get; private set; }
        public bool FishVulnerable { get; private set; }
        public Vector2 BaitVelocityDirection { get; private set; }

        protected Vector2 _playerSteerDir;
        protected float _steerDir;
        protected int _fishTendencySide;

        protected FishingArea _callerArea;
        protected Fish _fish;
        protected AnimalInfo FishInfo => _fish.Info;

        private void Awake()
        {
            _fishingControls = new FishingControls();
            _fishingControls.Rod.Reel.performed += ctx => UpdateReelState();
            _fishingControls.Rod.Reel.canceled += ctx => UpdateReelState();

            _playerControls = new PlayerControls();
            _playerControls.Movement.Look.performed += ctx => _playerSteerDir = ctx.ReadValue<Vector2>();
        }

        private void Update()
        {
            UpdateAction?.Invoke();
            SandboxUpdateAction?.Invoke();
        }

        public void Init(Fish fish, FishingArea callerArea)
        {
            _staminaPerlinSeed = UnityEngine.Random.Range(0, 1000);
            _steerPerlinSeed = UnityEngine.Random.Range(0, 1000);

            _fishTendencySide = UnityEngine.Random.Range(0.0f, 1.0f) >= 0.5f ?
                -1 : 1;

            _callerArea = callerArea;

            _fish = fish;
            _fishStamina = fish.Info.CatchingValues.Stamina;

            _reelCount = -1; // -1 prevents initial reel from counting
            _reelsRequired = fish.Info.CatchingValues.Stamina;

            OnFishBiteAction = StartCatchingBehaviour;

            ReelPressedAction = ReelIn;
            OnInit();
        }

        #region Sandbox
        protected virtual void OnInit()
        {
            OnFishBite();
        }

        protected virtual void DecreaseFishStamina()
        {
            _fishStamina -= GetStaminaDecreaseValue();
            DoFishMovement();

            if (_fishStamina <= NextWindow)
            {
                ReelWindow();
            }
        }

        protected virtual void InitialReelWindow()
        {
            FishVulnerable = true;
            _reelInWindowTimer = FishInfo.CatchingValues.ReelWindow + 1;
            ActiveCatchingAction = WhileOnInitialReelInWindow;
            OnReelWindow?.Invoke();
            BaitVelocityDirection = new Vector2(BaitVelocityDirection.x, 1);
            Debug.LogWarning("Waiting Initial Reel!");
        }

        protected virtual void WhileOnInitialReelInWindow()
        {
            _reelInWindowTimer -= Time.deltaTime;
            DoSidewaysFishMovement();

            if (_reelInWindowTimer <= 0)
            {
                _fishStamina = NextWindow + 1;
                FishVulnerable = false;
                Debug.LogWarning("Initial Reel window missed, fish got away...");
                ActiveCatchingAction = DecreaseFishStamina;
                Fail(true);
            }
        }

        protected virtual void ReelWindow()
        {
            FishVulnerable = true;
            _reelInWindowTimer = FishInfo.CatchingValues.ReelWindow + (_reelCount * FishInfo.CatchingValues.ReelWindowIncrease);
            ActiveCatchingAction = WhileOnReelInWindow;
            OnReelWindow?.Invoke();
            Debug.LogWarning("Reel window!");
        }
        protected virtual void WhileOnReelInWindow()
        {
            _reelInWindowTimer -= Time.deltaTime;
            DoSidewaysFishMovement();

            if (_reelInWindowTimer <= 0)
            {
                _fishStamina = NextWindow + 1;
                FishVulnerable = false;
                Debug.LogWarning("Reel window missed...");
                Fail();
                OnReelWindowMissed?.Invoke();
                ActiveCatchingAction = DecreaseFishStamina;
            }
        }

        protected virtual float GetStaminaDecreaseValue()
        {
            float randomStaminaFactor = (_fishStamina * _staminaPerlinSeed) /
                (FishInfo.CatchingValues.Stamina * _staminaPerlinSeed);
            float staminaPerlin = Mathf.PerlinNoise(randomStaminaFactor, randomStaminaFactor);

            return Time.deltaTime * staminaPerlin;
        }

        protected virtual void DoFishMovement()
        {
            DoSidewaysFishMovement();
            DoForwardFishMovement();
        }

        protected virtual void DoForwardFishMovement()
        {
            float randomPercentageFactor = (_fishStamina * _steerPerlinSeed) /
                (FishInfo.CatchingValues.Stamina * _steerPerlinSeed);
            float forwardPerlin = Mathf.PerlinNoise(randomPercentageFactor * 5, randomPercentageFactor * 5);
            float forward = forwardPerlin * (FishStaminaPercentage + 0.1f);
            Vector3 newPosition = _callerArea.BaitTransform.position +
                _callerArea.BaitTransform.forward * (forward * Time.deltaTime);

            _callerArea.BaitTransform.position = newPosition;

            BaitVelocityDirection = new Vector2(BaitVelocityDirection.x, forward);
        }

        protected virtual void DoSidewaysFishMovement()
        {
            float sidewaysPerlin = Mathf.PerlinNoise((Time.time + _steerPerlinSeed) * 5, (Time.time + _steerPerlinSeed) * 5);

            // Normalize result
            _steerDir = (sidewaysPerlin - 0.5f) * 2;

            BaitVelocityDirection = new Vector2(_steerDir * _fishTendencySide, BaitVelocityDirection.y);

            float sideways = _steerDir * 3;
            sideways *= Time.deltaTime;

            Vector3 newPosition = _callerArea.BaitTransform.position +
                (_callerArea.BaitTransform.right * sideways) * _fishTendencySide;

            _callerArea.BaitTransform.position = newPosition;
        }

        protected virtual void ReelIn()
        {
            if (FishVulnerable)
            {
                _reelCount++;
                FishVulnerable = false;
                OnReelSuccess?.Invoke();
                Debug.Log("Reel window taken!");
                _fishTendencySide *= -1;
                BaitVelocityDirection = new Vector2(BaitVelocityDirection.x, -1);

                if (_reelCount == _reelsRequired)
                {
                    UpdateAction = null;
                    //! move towards player and not back in the future
                    LeanTween.move(
                            _callerArea.BaitTransform.gameObject,
                            _callerArea.BaitTransform.position - (_callerArea.BaitTransform.forward * (1.3f - FishStaminaPercentage)),
                            .7f * (1.3f - FishStaminaPercentage))
                        .setEaseOutCirc()
                        .setOnComplete(FishCaught);
                }
                else
                {
                    //! move towards player and not back in the future
                    LeanTween.move(
                            _callerArea.BaitTransform.gameObject,
                            _callerArea.BaitTransform.position - (_callerArea.BaitTransform.forward * (1.3f - FishStaminaPercentage)),
                            .7f * (1.3f - FishStaminaPercentage))
                        .setEaseOutBack()
                        .setOnComplete(RestartStaminaDrain);
                }
            }
            else
            {
                Fail();
                OnReelFail?.Invoke();
            }

            void RestartStaminaDrain()
            {
                ActiveCatchingAction = DecreaseFishStamina;
            }

            void FishCaught()
            {
                EndFishing(true);
            }
        }
        #endregion

        #region Doesnt Matter

        private void OnFishBite()
        {
            OnFishBiteAction?.Invoke();
            Debug.LogWarning("We got a bite!");
            InitialReelWindow();
        }

        private void StartCatchingBehaviour()
        {
            UpdateAction = CatchingBehaviour;
        }

        private void CatchingBehaviour()
        {
            ActiveCatchingAction?.Invoke();
        }

        private void Fail(bool forceFinish = false)
        {
            Fails++;
            if (Fails > FishInfo.CatchingValues.FailAttempts || forceFinish)
                EndFishing(false);
            else

                Debug.Log("Failed reel...\n remaining fails: " + (FishInfo.CatchingValues.FailAttempts - Fails));
        }

        protected void EndFishing(bool success)
        {
            OnCatchEnded?.Invoke(success);
            _callerArea.FishingEnd(success);
        }

        private void UpdateReelState()
        {
            _reelPressed = !_reelPressed;

            if (_reelPressed) ReelPressedAction?.Invoke();
            else if (!_reelPressed) ReelReleasedAction?.Invoke();
        }

        private void OnEnable()
        {
            _fishingControls.Enable();
        }

        private void OnDisable()
        {
            _fishingControls.Disable();
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_callerArea.BaitTransform.position, BaitVelocityDirection);
        }

        private Action UpdateAction;

        protected Action SandboxUpdateAction;
        protected Action OnFishBiteAction;

        protected Action OnReelSuccess;
        protected Action OnReelFail;

        protected Action OnReelWindow;
        protected Action OnReelWindowMissed;

        private Action ActiveCatchingAction;

        private Action ReelPressedAction;
        private Action ReelReleasedAction;

        // passes true if successful, false if not
        protected Action<bool> OnCatchEnded;
    }
}