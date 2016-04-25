using UnityEngine;
using CapitalistTools;
using BigNumber;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string rateGameUrl;                      //Url adress to your game
    public UIManager ui;                            //reference to UIManager
    public AudioManager audioManager;               //reference to AudioManager
    public GameObject[] particles;                  //0 = money particle, 1 = click particle

    private StdBigNumber money;                     //the main money is here
    private StdBigNumber earnedOffline;             //this is used to store money earned offline at startup

    //Used for updating all the ui buttons, they grey out when you can or can't afford business, upgrade..
    public delegate void MoneyChange();
    public event MoneyChange onMoneyChanged;

    void Awake()
    {
        instance = this;                            //Set instance to this, so that you can access this script from anywhere
        MoneyTools.LoadScales();                    //Load scales (number names like Million, Trillion, ect..) from xml file
        money = MoneyTools.LoadMoney();             //Load saved amount of money from binary file

        //Prepare everything for UI
        ui.UpdateMoneyText(MoneyTools.GetFormatedMoneyText(money), MoneyTools.GetScaleName(money));
        LoadBusinessInfoAndSetupUI();
        LoadManagersAndSetupUI();
        LoadUpgradesAndSetupUI();

        AddMoney(0);                                //Just to activate all listening buttons, waiting for a change
    }

    #region Load All UI Stuff
    //Load all scriptableObjects from resources folder and Load their runtime states from the binary files
    private void LoadBusinessInfoAndSetupUI()
    {
        earnedOffline = new StdBigNumber(0);
        System.TimeSpan t = CapitalistTools.TimeTools.TimeSpentOffline();       
        Object[] b = Resources.LoadAll("Businesses/", typeof(BusinessInfo));
        if (b.Length > 0)
            foreach (Object o in b) //For all businesses
            {
                BusinessInfo info = o as BusinessInfo;
                info.state = CapitalistTools.States.LoadBusinessState(info.name);
                if (info.state == null)
                    info.Reset();
                //if business is owned, calculate how much money earned while being offline
                if (info.state.currentTime > 0.0f && info.state.isOwned)
                {
                    int timesLooped = 0;
                    float newTime = 0.0f;
                    info.state.currentTime += (float)t.TotalSeconds;
                    if (info.state.hasManager)
                    {
                        newTime = info.state.currentTime % info.state.revenueProgressBarTime;
                        timesLooped = (int)(info.state.currentTime / info.state.revenueProgressBarTime);
                    }
                    else if(info.state.hasManager == false)
                    {
                        if (info.state.currentTime > info.state.revenueProgressBarTime - 0.1f)
                        {
                            timesLooped = 1;
                            newTime = 0.0f;
                        }
                        else
                            newTime = info.state.currentTime;
                    }
                    info.state.currentTime = newTime;
                    StdBigNumber earn = new StdBigNumber(info.GetCurrentRevenue());
                    earn *= timesLooped;
                    earnedOffline += earn;
                }
                ui.AddBusinessToUI(info);
            }
        money += earnedOffline;         //Add total offline earnings from all businesses
        ui.SortBusinessesByCost();      //Sort businesses by cost so the cheapest shows up at top (in UI panel)
    }
    private void LoadUpgradesAndSetupUI()
    {
        Object[] u = Resources.LoadAll("Upgrades/", typeof(Upgrade));
        if (u.Length > 0)
            foreach (Object o in u)
            {
                Upgrade up = o as Upgrade;
                up.state = CapitalistTools.States.LoadUpgradeState(up.name);
                ui.AddUpgradeToUI(up);
            }
    }
    private void LoadManagersAndSetupUI()
    {
        Object[] m = Resources.LoadAll("Managers/", typeof(Manager));
        if (m.Length > 0)
            foreach (Object o in m)
            {
                Manager mg = o as Manager;
                mg.state = CapitalistTools.States.LoadManagerState(mg.name);
                ui.AddManagerToUI(mg);
            }
    }
    #endregion

    //Only for particles
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPos.z = -2;
            ParticleAt(1, screenPos);
        }
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
            Application.Quit();
    }
    public void ParticleAt(int index, Vector3 position)
    {
        ParticleSystem ps = particles[index].GetComponent<ParticleSystem>();
        if (ps.isPlaying)
            ps.Stop();
        particles[index].transform.position = position;
        ps.Play();
    }

    //When player quits the game the current time is saved as well as amount of money
    //Next time the player opens the game, this data is persisted
    void Save()
    {
        TimeTools.SaveTime();                       //Save current time on exit
        MoneyTools.SaveMoney(money);                //Save money
    }
    void OnApplicationFocus(bool focused)
    {
        if (!focused)
            Save();
    }
    void OnApplicationQuit()
    {
        Save();
    }

    #region Money Related Stuff
    public StdBigNumber HowMuchEarnedOffline()
    {
        return earnedOffline;
    }
    public bool HasEnoughMoney(string _money)
    {
        StdBigNumber number = new StdBigNumber(_money);
        if (!money.ISmaller(number))
            return true;
        else
            return false;
    }
    public void LoseMoney(string _amount)
    {
        StdBigNumber number = new StdBigNumber(_amount);
        money -= number;
        ui.UpdateMoneyText(MoneyTools.GetFormatedMoneyText(money), MoneyTools.GetScaleName(money));
        if(onMoneyChanged != null)
            onMoneyChanged();
    }
    public void AddMoney(string _amount)
    {
        StdBigNumber number = new StdBigNumber(_amount);
        money += number;
        ui.UpdateMoneyText(MoneyTools.GetFormatedMoneyText(money), MoneyTools.GetScaleName(money));
        if (onMoneyChanged != null)
            onMoneyChanged();
    }
    public void AddMoney(StdBigNumber _amount)
    {
        money += _amount;
        ui.UpdateMoneyText(MoneyTools.GetFormatedMoneyText(money), MoneyTools.GetScaleName(money));
        if (onMoneyChanged != null)
            onMoneyChanged();
    }
    #endregion
    public void CallOnMoneyChanged()
    {
        onMoneyChanged();
    }
    public void RateGame()
    {
        if (string.IsNullOrEmpty(rateGameUrl))
            return;
        Application.OpenURL(rateGameUrl);
    }
}
