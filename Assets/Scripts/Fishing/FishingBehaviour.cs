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

        private float _timeToFishBite;
        private float _fishStamina;
        private float _getawayCount;

        private float _reelInWindowTimer;
        private int _reelsRequired;
        private int _reelCount;

        protected bool FishVulnerable { get; private set; }

        protected FishingArea _containingArea;
        protected Animal _fish;

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

        public void Init(Animal fish, FishingArea callerArea)
        {
            _containingArea = callerArea;

            _fish = fish;
            _fishStamina = fish.CatchingValues.Stamina;
            _timeToFishBite = UnityEngine.Random.Range(
                fish.CatchingValues.WaitWindow.x,
                fish.CatchingValues.WaitWindow.y);

            _reelCount = 0;
            _reelsRequired = fish.CatchingValues.Stamina;
            Debug.Log(_reelsRequired);

            OnFishBiteAction = StartCatchingBehaviour;

            ReelPressedAction = ReelIn;
            OnInit();
        }

        public void GlupGlupInit(FishingArea callerArea)
        {
            Init(Resources.Load<Animal>("Fauna/Aquatic/GlupGlup"), callerArea);
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
            _fishStamina -= Time.deltaTime;
            Debug.Log(_fishStamina);
            if (_fishStamina <= (_fish.CatchingValues.Stamina - 1) - _reelCount)
            {
                FishVulnerable = true;
                _reelInWindowTimer = _fish.CatchingValues.ReelWindow + (_reelCount * _fish.CatchingValues.ReelWindowIncrease);
                ActiveCatchingAction = WhileOnReelInWindow;
            }
            if (_fishStamina <= 0)
            {
                UpdateAction = null;
                OnCatchEnded?.Invoke(true);
                _containingArea.FishingEnd(true);
            }
        }

        protected virtual void WhileOnReelInWindow()
        {
            _reelInWindowTimer -= Time.deltaTime;

            if (_reelInWindowTimer <= 0)
            {
                FishVulnerable = false;
                OnReelWindowMissed?.Invoke();
                UpdateAction = DecreaseFishStamina;
            }

        }

        protected virtual void ReelIn()
        {
            if (FishVulnerable)
            {
                UpdateAction = DecreaseFishStamina;
                OnReelSuccess?.Invoke();
            }
            else 
            {
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
        }

        private void StartCatchingBehaviour()
        {
            UpdateAction = CatchingBehaviour;
        }

        private void CatchingBehaviour()
        {
            ActiveCatchingAction?.Invoke();
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

        protected Action OnReelWindowMissed;

        private Action ActiveCatchingAction;

        private Action ReelPressedAction;
        private Action ReelReleasedAction;

        // passes true if successful, false if not
        protected Action<bool> OnCatchEnded;
    }
}