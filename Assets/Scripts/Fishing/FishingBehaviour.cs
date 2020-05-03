using System;
using System.Collections;
using Fauna;
using Fishing.Area;
using UnityEngine;

namespace Fishing
{
    public class FishingBehaviour : MonoBehaviour
    {
        private const float GETAWAY_THRESHOLD = 1;
        private FishingControls _controls;
        private bool _reelPressed;

        private int _perlinSeed;

        private float _timeToFishBite;
        private float _fishStamina;
        private float _getawayCount;

        private float _reelInWindowTimer;
        private int _reelsRequired;
        private int _reelCount;

        private float NextWindow => (_fish.CatchingValues.Stamina - 1) - _reelCount;

        public float FishStaminaPercentage => _fishStamina / _fish.CatchingValues.Stamina;
        public int Fails { get; private set; }
        public bool FishVulnerable { get; private set; }

        protected FishingArea _containingArea;
        protected AnimalInfo _fish;

        private void Awake()
        {
            _controls = new FishingControls();
            _controls.Rod.Reel.performed += ctx => UpdateReelState();
            _controls.Rod.Reel.canceled += ctx => UpdateReelState();
        }

        private void Update()
        {
            UpdateAction?.Invoke();
            SandboxUpdateAction?.Invoke();
        }

        public void Init(AnimalInfo fish, FishingArea callerArea)
        {
            _perlinSeed = UnityEngine.Random.Range(0, int.MaxValue);
            _containingArea = callerArea;

            _fish = fish;
            _fishStamina = fish.CatchingValues.Stamina;
            _timeToFishBite = UnityEngine.Random.Range(
                fish.CatchingValues.WaitWindow.x,
                fish.CatchingValues.WaitWindow.y);

            _reelCount = 0;
            _reelsRequired = fish.CatchingValues.Stamina;

            OnFishBiteAction = StartCatchingBehaviour;

            ReelPressedAction = ReelIn;
            OnInit();
        }

        #region Sandbox
        protected virtual void OnInit()
        {
            StartBiteWait();
        }

        protected void StartBiteWait()
        {
            StartCoroutine(CWaitForFishBite());
        }

        protected virtual void DecreaseFishStamina()
        {
            float _factor = (_fishStamina * _perlinSeed) /
                (_fish.CatchingValues.Stamina * _perlinSeed);
            float _staminaPerlin = Mathf.PerlinNoise(_factor, _factor);
            _fishStamina -= Time.deltaTime * _staminaPerlin;

            if (_fishStamina <= NextWindow)
            {
                FishVulnerable = true;
                _reelInWindowTimer = _fish.CatchingValues.ReelWindow + (_reelCount * _fish.CatchingValues.ReelWindowIncrease);
                ActiveCatchingAction = WhileOnReelInWindow;
                OnReelWindow?.Invoke();
                Debug.LogWarning("Reel window!");
            }
        }

        protected virtual void WhileOnReelInWindow()
        {
            _reelInWindowTimer -= Time.deltaTime;

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

        protected virtual void ReelIn()
        {
            if (FishVulnerable)
            {
                _reelCount++;
                ActiveCatchingAction = DecreaseFishStamina;
                OnReelSuccess?.Invoke();
                Debug.Log("Reel window taken!");

                if (_reelCount == _reelsRequired)
                {
                    UpdateAction = null;
                    EndFishing(true);
                }
            }
            else
            {
                Fail();
                OnReelFail?.Invoke();
            }
        }
        #endregion

        #region Doesnt Matter
        private IEnumerator CWaitForFishBite()
        {
            yield return new WaitForSeconds(_timeToFishBite);
            OnFishBite();
        }

        private void OnFishBite()
        {
            ActiveCatchingAction = DecreaseFishStamina;
            OnFishBiteAction?.Invoke();
            Debug.LogWarning("We got a bite!");
        }

        private void StartCatchingBehaviour()
        {
            UpdateAction = CatchingBehaviour;
        }

        private void CatchingBehaviour()
        {
            ActiveCatchingAction?.Invoke();
        }

        private void Fail()
        {
            Fails++;
            if (Fails > _fish.CatchingValues.FailAttempts)
                EndFishing(false);
            else
                Debug.Log("Failed reel...\n remaining fails: " + (_fish.CatchingValues.FailAttempts - Fails));
        }

        protected void EndFishing(bool success)
        {
            OnCatchEnded?.Invoke(success);
            _containingArea.FishingEnd(success);
        }

        private void UpdateReelState()
        {
            _reelPressed = !_reelPressed;

            if (_reelPressed) ReelPressedAction?.Invoke();
            else if (!_reelPressed) ReelReleasedAction?.Invoke();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
        #endregion

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