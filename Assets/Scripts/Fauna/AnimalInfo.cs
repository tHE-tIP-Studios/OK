using System;
using UnityEngine;
using Clock;

namespace Fauna
{
    [CreateAssetMenu(menuName = "OK/Animal Info")]
    public class AnimalInfo : ScriptableObject
    {
        [SerializeField] private AnimalSpecie _specie = default;
        [SerializeField] private string _name = default;
        [SerializeField] private string _briefDescripion = default;

        [TextArea]
        [SerializeField] private string _detailedDescripion = default;

        [SerializeField] private AvailabilityValues _availability = default;

        [SerializeField] private CatchValues _catchingValues = default;

        // Add serialized struct for the cooking values

        public AnimalSpecie Specie => _specie;
        public string Name => _name;
        public string BriefDescripion => _briefDescripion;
        public string DetailedDescripion => _detailedDescripion;
        public AvailabilityValues Availability => _availability;
        public CatchValues CatchingValues => _catchingValues;
    }
}