using UnityEngine;

[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
    [HideInInspector]
    public UpgradeState state;
    public string cost;
    public BusinessInfo appliesToBusiness;
    public float revenueMultiplier = 1.0f;
}
[System.Serializable]
public class UpgradeState
{
    [SerializeField]
    public bool isUnlocked;
    public UpgradeState()
    {
        isUnlocked = false;
    }
}
