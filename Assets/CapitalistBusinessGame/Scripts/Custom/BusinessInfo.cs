using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class BusinessInfo : ScriptableObject
{
    [HideInInspector]
    public BusinessState state;

    [Header("Default Starting Stats")]
    public string costsToOwn;
    [Space()]
    public string startingRevenue;

    //How much the next investment increases
    public float coefficient;

    public string startingInvestCost;

    //How long to wait until you get revenue / profit
    public float startingTime;
    [Space()]
    public Sprite icon;
    [Space()]
    [Header("List of Unlocks for this business")]
    public List<Unlock> unlocks;

    public void Reset() //Reset to its default state
    {
        state = new BusinessState();
        state.isOwned = false;
        state.hasManager = false;
        state.currentLevel = 0;
        state.currentUnlock = 0;
        state.currentRevenue = startingRevenue;
        state.currentRevenueMultiplier = 0.0f;
        state.currentInvestCost = startingInvestCost;
        state.currentInvestMultiplier = 1.0f;
        state.revenueProgressBarTime = startingTime;
        state.currentTime = 0.0f;
    }
    public string GetCurrentRevenue()   //Get revenue multiplied by bonus multiplier
    {
        if (state.currentRevenueMultiplier == 0.0f)
            return state.currentRevenue;
        BigNumber.StdBigNumber currRev = new BigNumber.StdBigNumber(state.currentRevenue);
        currRev *= (int)state.currentRevenueMultiplier;
        return currRev.ToString();
    }
}

[System.Serializable]
public class BusinessState
{
    [SerializeField]
    public bool isOwned = false;
    [SerializeField]
    public bool hasManager = false;
    [SerializeField]
    public int currentLevel = 0;
    [SerializeField]
    public int currentUnlock = 0;
    [SerializeField]
    public string currentRevenue;
    [SerializeField]
    public float currentRevenueMultiplier = 0.0f;
    [SerializeField]
    public string currentInvestCost;
    [SerializeField]
    public float currentInvestMultiplier = 1.0f;
    [SerializeField]
    public float revenueProgressBarTime;
    [SerializeField]
    public float currentTime;

    public BusinessState() { }
}
