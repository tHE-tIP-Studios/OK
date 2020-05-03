using System.Collections;
using System.Collections.Generic;
using Fauna;
using Fauna.Animals;
using UnityEngine;

namespace Fishing.Area
{
    public static class Spawner
    {
        public static GameObject TemplateFish { get; private set; }

        static Spawner()
        {
            TemplateFish = Resources.Load<GameObject>("Prefabs/Fauna/Fish Template");
        }

        public static IEnumerable<Fish> SpawnFish(FishingArea area)
        {
            // Use randomization and time phases in the future
            Debug.Log("a");
            AnimalInfo chosenInfo = Resources.Load<AnimalInfo>("Fauna/Aquatic/GlupGlup");

            for (int i = 0; i < area.Capacity; i++)
            {
                Fish newFish =
                    GameObject.Instantiate(TemplateFish).GetComponent<Fish>();
                newFish.gameObject.name = chosenInfo.name;

                newFish.transform.position = GetStartPosInArea(area);
                newFish.transform.SetParent(area.FishParent);
                newFish.Init(chosenInfo);
                yield return newFish;
            }
        }

        private static Vector3 GetStartPosInArea(FishingArea area)
        {
            Vector3 startPos = new Vector3(
                Random.Range(-area.MaxDistanceFromCenter, area.MaxDistanceFromCenter),
                Random.Range(area.transform.position.y - 3.0f, area.transform.position.y - .5f),
                Random.Range(-area.MaxDistanceFromCenter, area.MaxDistanceFromCenter)
            );

            if (!area.IsInside(startPos))
                return GetStartPosInArea(area);
            else
                return startPos;
        }
    }
}