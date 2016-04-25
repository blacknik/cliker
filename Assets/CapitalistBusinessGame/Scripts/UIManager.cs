using UnityEngine;
using UnityEngine.UI;
using System;
using CapitalistTools;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Money text at the top")]
    public Text moneyText;
    public Text scaleText;

    [Header("Prefab to Instantiate")]
    public GameObject businessUIElementPrefab;
    public GameObject managerUIElementPrefab;
    public GameObject unlockUIElementPrefab;
    public GameObject upgradeUIElementPrefab;

    [Header("Containers for items")]
    public GameObject businessesContainer;
    public GameObject managersContainer;
    public GameObject unlocksContainer;
    public GameObject upgradesContainer;

    [Header("Startup Window Stuff")]
    public Text timeSpentOffline;
    public Text earnedWhileOffline;

    [Header("All UI Windows")]
    public GameObject startUpWindow;
    public GameObject managerWindow;
    public GameObject unlocksWindow;
    public GameObject upgradesWindow;
    public GameObject playerInfoWindow;

    private List<BusinessUIElement> businessUIElements = new List<BusinessUIElement>();

    void Start()
    {
        startUpWindow.SetActive(false);
        managerWindow.SetActive(false);
        unlocksWindow.SetActive(false);
        upgradesWindow.SetActive(false);
        playerInfoWindow.SetActive(false);
        ShowStartUpScreen();
    }
    public void UpdateMoneyText(string _money, string _scale)
    {
        moneyText.text = _money;
        scaleText.text = _scale;
    }
    public void UpdateBusinessRevText()
    {
        for (int i = 0; i < businessUIElements.Count; i++)
            if (businessUIElements[i].info.state.isOwned)
                businessUIElements[i].UpdateRevenueText();
    }
    public void AddBusinessToUI(BusinessInfo b)
    {
        GameObject prefab = Instantiate(businessUIElementPrefab) as GameObject;
        BusinessUIElement ui = prefab.GetComponent<BusinessUIElement>();
        ui.info = b;
        businessUIElements.Add(ui);
        AddUnlockToUI(ui);
    }
    public void AddUnlockToUI(BusinessUIElement b)
    {
        GameObject unlockEl = Instantiate(unlockUIElementPrefab) as GameObject;
        unlockEl.transform.SetParent(unlocksContainer.transform, false);
        b.currentUnlockUIElement = unlockEl.GetComponent<UnlockUIElement>();
        b.currentUnlockUIElement.SetupUI(b.info, b.info.state.currentUnlock);
    }
    public void AddManagerToUI(Manager m)
    {
        if (m.state.isHired)
            return;
        GameObject prefab = Instantiate(managerUIElementPrefab) as GameObject;
        ManagerUIElement ui = prefab.GetComponent<ManagerUIElement>();
        ui.gameObject.transform.SetParent(managersContainer.transform, false);
        ui.SetupUI(m);
    }
    public void AddUpgradeToUI(Upgrade u)
    {
        if (u.state.isUnlocked)
            return;
        GameObject prefab = Instantiate(upgradeUIElementPrefab) as GameObject;
        UpgradesUIElement ui = prefab.GetComponent<UpgradesUIElement>();
        ui.gameObject.transform.SetParent(upgradesContainer.transform, false);
        ui.SetupUI(u);
    }
    public void SortBusinessesByCost()
    {
        businessUIElements.Sort(delegate (BusinessUIElement a, BusinessUIElement b)
        {
            return (new BigNumber.StdBigNumber(a.info.costsToOwn)).CompareTo(new BigNumber.StdBigNumber(b.info.costsToOwn));
        });
        foreach(BusinessUIElement ui in businessUIElements)
        {
            ui.gameObject.transform.SetParent(businessesContainer.transform, false);
            ui.SetupElement();
        }
    }
    public void ShowStartUpScreen()
    {
        startUpWindow.SetActive(!startUpWindow.activeSelf);
        if(startUpWindow.activeSelf)
        {
            TimeSpan t = TimeTools.TimeSpentOffline();
            timeSpentOffline.text = Mathf.Floor((float)t.TotalHours) + "h " + t.Minutes + "m " + t.Seconds + "s";
            BigNumber.StdBigNumber earnedOffline = GameManager.instance.HowMuchEarnedOffline();
            earnedWhileOffline.text = MoneyTools.GetFormatedMoneyText(earnedOffline) + " " + MoneyTools.GetScaleName(earnedOffline);
        }
    }
    public void ShowUpgradesWindow()
    {
        GameManager.instance.audioManager.PlaySound(SoundType.CLICK);
        upgradesWindow.SetActive(!upgradesWindow.activeSelf);
        GameManager.instance.CallOnMoneyChanged();
    }
    public void ShowUnlocksWindow()
    {
        GameManager.instance.audioManager.PlaySound(SoundType.CLICK);
        unlocksWindow.SetActive(!unlocksWindow.activeSelf);
    }
    public void ShowManagersWindow()
    {
        GameManager.instance.audioManager.PlaySound(SoundType.CLICK);
        managerWindow.SetActive(!managerWindow.activeSelf);
        GameManager.instance.CallOnMoneyChanged();
    }
    public void ShowPlayerInfoWindow()
    {
        GameManager.instance.audioManager.PlaySound(SoundType.CLICK);
        playerInfoWindow.SetActive(!playerInfoWindow.activeSelf);
    }
}