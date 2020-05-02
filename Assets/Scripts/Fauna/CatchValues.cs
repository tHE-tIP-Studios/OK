using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CatchValues
{
    [Range(0, 1)]
    [SerializeField] private float _spawnChance;
    [Range(0, 1)]
    [SerializeField] private float _catchDifficulty;
}