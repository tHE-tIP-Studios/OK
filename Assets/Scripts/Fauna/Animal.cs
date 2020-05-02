using System;
using UnityEngine;
using Clock;

namespace Fauna
{
    [CreateAssetMenu(menuName = "OK/Animal")]
    public class Animal : ScriptableObject
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
    }
}