using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CatchValues
{    
    [Range(2, 20)]
    [SerializeField] private int _stamina;
    [SerializeField] private int _failAttempts;
    [SerializeField] private float _reelInWindow;
    [SerializeField] private float _windowIncreasePerReel;
    [SerializeField] private float _baseBaitInterest;

    public int Stamina => _stamina;
    public int FailAttempts => _failAttempts;
    public float ReelWindow => _reelInWindow;
    public float ReelWindowIncrease => _windowIncreasePerReel;
    public float BaitInterest => _baseBaitInterest;

    public CatchValues(int stamina, int fails, float reelWindow, float windIncrease, float interest)
    {
        _stamina = stamina;
        _failAttempts = fails;
        _reelInWindow = reelWindow;
        _windowIncreasePerReel = windIncrease;
        _baseBaitInterest = interest;
    }
}