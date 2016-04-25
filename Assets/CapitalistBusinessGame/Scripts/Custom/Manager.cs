using System;
using UnityEngine;

[CreateAssetMenu]
public class Manager : ScriptableObject
{
    [HideInInspector]
    public ManagerState state;
    public Sprite icon;
    public string costsToHire;
    public BusinessInfo runsBusiness;
}

[Serializable]
public class ManagerState
{
    [SerializeField] public bool isHired = false;
    public ManagerState()
    {
        isHired = false;
    }
}
