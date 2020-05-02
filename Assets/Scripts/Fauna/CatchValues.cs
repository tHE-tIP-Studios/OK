using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CatchValues
{    
    [Range(2, 10)]
    [SerializeField] private int _stamina;
    [SerializeField] private int _failAttempts;
    [SerializeField] private float _reelInWindow;
    [SerializeField] private float _windowIncreasePerReel;
    [SerializeField] private Vector2 _biteWaitWindow;

    public int Stamina => _stamina;
    public int FailAttempts => _failAttempts;
    public float ReelWindow => _reelInWindow;
    public float ReelWindowIncrease => _windowIncreasePerReel;
    public Vector2 WaitWindow => _biteWaitWindow;
}