using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clock;

namespace Fauna
{
    [System.Serializable]
    public struct AvailabilityValues
    { 
        [SerializeField] private Month _availableMonths;
        [SerializeField] private DayPhase _availablePhases;

        public Month Months => _availableMonths;
        public DayPhase Phases => _availablePhases;
    }
}