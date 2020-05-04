using System.Collections;
using System.Collections.Generic;
using Clock;
using UnityEngine;

namespace Fauna
{
    [System.Serializable]
    public struct AvailabilityValues
    {
        [SerializeField] private Month _availableMonths;
        [SerializeField] private DayPhase _availablePhases;

        [Range(0, 1)]
        [SerializeField] private float _spawnChance;
        
        [Tooltip("In Seconds, is the time that the animal will stay in overwold" +
            "before despawning.")]
        [SerializeField] private float _lifespan;

        public Month Months => _availableMonths;
        public DayPhase Phases => _availablePhases;

        public float SpawnChance => _spawnChance;
        public float Lifespan => _lifespan;
    }
}