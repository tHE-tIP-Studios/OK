using System;
using Clock;
using Fishing.Area;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Fauna
{
    [CreateAssetMenu(menuName = "OK/Animal Info")]
    public class AnimalInfo : ScriptableObject
    {
        [SerializeField] private GameObject _fishGFX = default;
        [SerializeField] private int _id = default;
        [SerializeField] private AnimalSpecie _specie = default;
        [SerializeField] private FishingAreaType _location = default;
        [SerializeField] private string _name = default;
        [SerializeField] private string _subtitle = default;

        [TextArea]
        [SerializeField] private string _detailedDescripion = default;

        [SerializeField] private AvailabilityValues _availability = default;

        [SerializeField] private CatchValues _catchingValues = default;

        // Add serialized struct for the cooking values

        public GameObject FishGFX => _fishGFX;
        public AnimalSpecie Specie => _specie;
        public string Name => _name;
        public string Subtitle => _subtitle;
        public string DetailedDescripion => _detailedDescripion;
        public AvailabilityValues Availability => _availability;
        public CatchValues CatchingValues => _catchingValues;

        public static void CreateNewAsset(RawInfoHolder raw)
        {
            AnimalInfo newAnimal =
                ScriptableObject.CreateInstance<AnimalInfo>();

            newAnimal._id = raw.Id;
            newAnimal._name = raw.Name;
            newAnimal._subtitle = raw.Subtitle;
            newAnimal._detailedDescripion = raw.Description;
            newAnimal._specie = (AnimalSpecie) Enum.Parse(typeof(AnimalSpecie), raw.Specie);
            newAnimal._location = (FishingAreaType) Enum.Parse(typeof(FishingAreaType), raw.Location);

            CatchValues cValues = new CatchValues(int.Parse(raw.Stamina),
                int.Parse(raw.Fails),
                int.Parse(raw.ReelWindow) * 0.01f,
                int.Parse(raw.WindowIncrease) * 0.01f,
                int.Parse(raw.BaitInterest) * 0.01f);

            AvailabilityValues aValues = new AvailabilityValues(
                GetMonths(),
                GetPhases(),
                int.Parse(raw.SpawnChance) * 0.01f,
                int.Parse(raw.Lifespan));

            newAnimal._catchingValues = cValues;
            newAnimal._availability = aValues;

            string path = "Assets/Resources/Fauna Info/Aquatic/" + raw.Specie + '/';
            string file = newAnimal._name + ".asset";

            AnimalInfo[] foundAssets =
                Resources.LoadAll<AnimalInfo>("Fauna Info/Aquatic/" + raw.Specie);

            bool flag = false;
            foreach (AnimalInfo i in foundAssets)
                if (i._name == newAnimal._name)
                {
                    newAnimal._fishGFX = i.FishGFX;
                    AssetDatabase.DeleteAsset(path + file);
                    Debug.Log("Found duplicate of " + newAnimal.Name + "\nReplacing...");
                    flag = true;
                }

            if (!flag) Debug.Log("New Animal Added: " + newAnimal.Name);

            AssetDatabase.CreateAsset(newAnimal, path + file);

            Month GetMonths()
            {
                Month months = Month.NONE;
                string[] rawMonths = raw.Months.Split(' ');
                for (int i = 0; i < rawMonths.Length; i++)
                {
                    switch (int.Parse(rawMonths[i]))
                    {
                        case 1:
                            months |= Month.JANUARY;
                            break;
                        case 2:
                            months |= Month.FEBRUARY;
                            break;
                        case 3:
                            months |= Month.MARCH;
                            break;
                        case 4:
                            months |= Month.APRIL;
                            break;
                        case 5:
                            months |= Month.MAY;
                            break;
                        case 6:
                            months |= Month.JUNE;
                            break;
                        case 7:
                            months |= Month.JULY;
                            break;
                        case 8:
                            months |= Month.AUGUST;
                            break;
                        case 9:
                            months |= Month.SEPTEMBER;
                            break;
                        case 10:
                            months |= Month.OCTOBER;
                            break;
                        case 11:
                            months |= Month.NOVEMBER;
                            break;
                        case 12:
                            months |= Month.DECEMBER;
                            break;
                    }
                }
                return months;
            }

            DayPhase GetPhases()
            {
                DayPhase phases = DayPhase.NONE;
                string[] rawPhases = raw.Phases.Split(' ');
                for (int i = 0; i < rawPhases.Length; i++)
                {
                    switch (rawPhases[i])
                    {
                        case "EM":
                            phases |= DayPhase.Early_Morning;
                            break;
                        case "LM":
                            phases |= DayPhase.Late_Morning;
                            break;
                        case "EA":
                            phases |= DayPhase.Early_Afternoon;
                            break;
                        case "LA":
                            phases |= DayPhase.Late_Afternoon;
                            break;
                        case "EN":
                            phases |= DayPhase.Early_Night;
                            break;
                        case "LN":
                            phases |= DayPhase.Late_Night;
                            break;
                        case "NO":
                            phases |= DayPhase.Night_Owl;
                            break;
                    }
                }
                return phases;
            }
        }
    }
}