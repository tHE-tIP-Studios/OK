﻿using System;
using System.Collections.Generic;
using Clock;
using Clock.Events;
using UnityEngine;

namespace Fauna.Animals
{
    public abstract class Animal : MonoBehaviour
    {
        public const int ANIMAL_LAYER = 10;
        public const string ANIMAL_LAYER_NAME = "Animal";
        public AnimalInfo Info { get; private set; }
        public GameObject GFXObject { get; private set; }

        protected float LifespanLeft { get; private set; }
        protected SavedTime SpawnTime { get; private set; }
        protected bool TimeIsUp { get; private set; }

        private byte LifetimeFactor;
        private ClockEvent LifespanSpeedUpEvent;

        private void Awake()
        {
            gameObject.layer = ANIMAL_LAYER;
        }

        public virtual void Init(AnimalInfo info)
        {
            Info = info;

            SpawnTime = MainClock.NowTime;
            LifespanLeft = Info.Availability.Lifespan;
            LifetimeFactor = 1;

            GFXObject =
                Instantiate(info.FishGFX, transform);
            GFXObject.name = "GFX";

            LifespanSpeedUpEvent = new ClockEvent(
                SavedTime.NoTime,
                EventRepetitionType.Every_DayPhase,
                false,
                CheckCurrentPhase);

            LifespanSpeedUpEvent.InjectClockEvent();
        }

        private void Update()
        {
            DoLifespanCountdown();
            OnUpdate();
        }

        private void DoLifespanCountdown()
        {
            if (TimeIsUp) return;

            LifespanLeft -= Time.unscaledDeltaTime * LifetimeFactor;

            if (LifespanLeft <= 0)
            {
                LifespanSpeedUpEvent?.UnsubscribeFromClockEvents();
                TimeIsUp = true;
            }
        }

        private void CheckCurrentPhase(SavedTime time)
        {
            if (!Info.Availability.Phases.HasFlag(time.DayPhase))
            {
                LifetimeFactor++;
                LifespanSpeedUpEvent?.UnsubscribeFromClockEvents();
                Debug.Log(name + " is out of its dayphase!\nDecreasing lifetime.");
            }
        }

        protected void Despawn()
        {
            OnDespawn?.Invoke();
            Destroy(gameObject);
        }

        protected abstract void OnUpdate();

        public Action OnDespawn;
    }
}