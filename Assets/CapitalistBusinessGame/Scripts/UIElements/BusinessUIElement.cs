using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BusinessUIElement : MonoBehaviour
{
    [HideInInspector]
    public BusinessInfo info;
    [HideInInspector]
    public UnlockUIElement currentUnlockUIElement;
    private float currentTime;
    private bool running = false;

    [Header("Not owned business")]
    public GameObject ownBusinessElement;
    public Text ownBusinessName;
    public Text ownBusinessFor;
    public Button ownBusinessButton;

    [Header("Owned business")]
    public GameObject businessUIElement;
    public Image icon;
    public Text currentLevelText;
    public Image levelProgressBar;
    public Button workButton;
    public Button investButton;
    public Text investCostText;
    public Text investScaleText;
    public Image revenueProgressBar;
    public Text revenueMoneyText;
    public Text timeLeftText;

    public void SetupElement()
    {
        currentTime = info.state.currentTime;
        if (currentTime > 0.0f)
            TryRunBusiness();
        if (info.state.isOwned == false)
        {
            businessUIElement.SetActive(false);
            ownBusinessElement.SetActive(true);

            ownBusinessName.text = info.name;
            ownBusinessFor.text = CapitalistTools.MoneyTools.GetFormatedMoneyText(new BigNumber.StdBigNumber(info.costsToOwn)) + " " + CapitalistTools.MoneyTools.GetScaleName(info.costsToOwn);

            GameManager.instance.onMoneyChanged += OwnBusinessButtonChange;
            ownBusinessButton.onClick.AddListener(new UnityAction(this.OwnThisBusiness));
        }
        else
        {
            ownBusinessElement.SetActive(false);
            businessUIElement.SetActive(true);

            icon.sprite = info.icon;
            UpdateCurrentLevelUI();
            UpdateTimeUI();
            UpdateRevenueText();
            UpdateRevenueProgressBarUI();
            UpdateInvestButton();

            GameManager.instance.onMoneyChanged += InvestButtonChange;

            workButton.onClick.AddListener(new UnityAction(this.TryRunBusiness));
            investButton.onClick.AddListener(new UnityAction(this.Invest));

            if (info.state.hasManager)
                TryRunBusiness();
        }
    }

    //Changes buttons interactability based on money
    void OwnBusinessButtonChange()
    {
        if (GameManager.instance.HasEnoughMoney(info.costsToOwn))
            ownBusinessButton.interactable = true;
        else
            ownBusinessButton.interactable = false;
    }
    void InvestButtonChange()
    {
        if (GameManager.instance.HasEnoughMoney(info.state.currentInvestCost))
            investButton.interactable = true;
        else
            investButton.interactable = false;
    }

    //Update specific Ui element
    public void UpdateRevenueText()
    {
        string currRev = info.GetCurrentRevenue();
        revenueMoneyText.text = CapitalistTools.MoneyTools.GetFormatedMoneyText(new BigNumber.StdBigNumber(currRev)) + " " + CapitalistTools.MoneyTools.GetScaleName(currRev);
    }
    void UpdateInvestButton()
    {
        investCostText.text = CapitalistTools.MoneyTools.GetFormatedMoneyText(new BigNumber.StdBigNumber(info.state.currentInvestCost));
        investScaleText.text = CapitalistTools.MoneyTools.GetScaleName(info.state.currentInvestCost);
    }
    void UpdateRevenueProgressBarUI()
    {
        revenueProgressBar.fillAmount = currentTime / info.state.revenueProgressBarTime;
    }
    void UpdateTimeUI()
    {
        float timeLeft = info.state.revenueProgressBarTime - currentTime;
        if (timeLeft < 5.0f)
            timeLeftText.text = timeLeft.ToString("F1") + "s";
        else
            timeLeftText.text = timeLeft.ToString("F0") + "s";
    }
    void UpdateCurrentLevelUI()
    {
        levelProgressBar.fillAmount =
        (float)(info.state.currentLevel - info.unlocks[info.state.currentUnlock - 1].levelToUnlock) / (float)(info.unlocks[info.state.currentUnlock].levelToUnlock - info.unlocks[info.state.currentUnlock - 1].levelToUnlock);
        if (info.state.currentLevel == info.unlocks[info.state.currentUnlock].levelToUnlock)
        {
            currentLevelText.text = info.state.currentLevel.ToString() + " (MAX)";
            investButton.interactable = false;
        }else
           currentLevelText.text = info.state.currentLevel.ToString();
    }

    //Set these functions to buttons
    void OwnThisBusiness()
    {
        if(GameManager.instance.HasEnoughMoney(info.costsToOwn))
        {
            Invest();
            info.state.isOwned = true;

            GameManager.instance.onMoneyChanged -= OwnBusinessButtonChange;
            GameManager.instance.onMoneyChanged += InvestButtonChange;
            GameManager.instance.LoseMoney(info.costsToOwn);
            ownBusinessElement.SetActive(false);
            businessUIElement.SetActive(true);

            icon.sprite = info.icon;
            UpdateCurrentLevelUI();
            UpdateTimeUI();
            UpdateRevenueText();
            UpdateRevenueProgressBarUI();
            UpdateInvestButton();

            workButton.onClick.AddListener(new UnityAction(this.TryRunBusiness));
            investButton.onClick.AddListener(new UnityAction(this.Invest));
        }
    }
    void Invest()
    {
        if (GameManager.instance.HasEnoughMoney(info.state.currentInvestCost) && info.state.currentUnlock <= info.unlocks.Count - 1 || info.state.currentLevel == 0)
        {
            if (info.state.currentLevel < info.unlocks[info.unlocks.Count-1].levelToUnlock)
            {
                //Add level
                info.state.currentLevel++;

                //Everytime you invest increase revenue
                BigNumber.StdBigNumber currentRev = new BigNumber.StdBigNumber(info.state.currentRevenue);
                BigNumber.StdBigNumber revToAdd = new BigNumber.StdBigNumber(info.startingRevenue);
                currentRev += revToAdd;
                info.state.currentRevenue = currentRev.ToString();
                UpdateRevenueText();

                GameManager.instance.audioManager.PlaySound(SoundType.INVEST);

                if (info.state.currentLevel >= info.unlocks[info.state.currentUnlock].levelToUnlock && info.state.currentLevel != 0 && info.state.currentUnlock < info.unlocks.Count - 1)
                {
                    info.state.currentUnlock++;
                    //Every unlock the revenue progress Bar Time shortens by a multiplier
                    info.state.revenueProgressBarTime /= info.unlocks[info.state.currentUnlock].timeMultiplier;
                    GameManager.instance.audioManager.PlaySound(SoundType.UNLOCK);

                    //Create new UI Element for current unlock
                    currentUnlockUIElement.transform.SetParent(null, false);
                    Destroy(currentUnlockUIElement.gameObject);
                    currentUnlockUIElement = null;
                    GameManager.instance.ui.AddUnlockToUI(this);
                }
                if(info.state.currentLevel > 1)
                    GameManager.instance.LoseMoney(info.state.currentInvestCost);

                //also recalculate invest cost
                BigNumber.StdBigNumber startInvest = new BigNumber.StdBigNumber(info.startingInvestCost);
                BigNumber.StdBigNumber newInvest = new BigNumber.StdBigNumber();

                //THIS CAUSED A BUG IN v1.0
                //newInvest += (int)Mathf.FloorToInt(startInvest * Mathf.Pow(info.coefficient, info.state.currentLevel));
                newInvest += (startInvest * (Mathf.FloorToInt(Mathf.Pow(info.coefficient, info.state.currentLevel) * 100))) / 100;

                info.state.currentInvestCost = newInvest.ToString();

                //Emit money particle
                GameManager.instance.ParticleAt(0, icon.transform.position);
                UpdateInvestButton();
                UpdateCurrentLevelUI();
            }
        }
    }

    void TryRunBusiness()
    {
        GameManager.instance.audioManager.PlaySound(SoundType.CLICK);
        if (running)
            return;
        running = true;
        //you have to click to work
        if (info.state.hasManager == false)
            StartCoroutine(Run());
        else //if it has manager, it loops automaticaly
            StartCoroutine(GenerateIncome());
    }
    //When you don't have a manager hired
    IEnumerator Run()
    {
        while(currentTime < info.state.revenueProgressBarTime)
        {
            currentTime += (Time.deltaTime + 0.05f);
            UpdateRevenueProgressBarUI();
            UpdateTimeUI();
            yield return new WaitForSeconds(0.05f);
        }
        //Cycle completed
        running = false;
        currentTime = 0.0f;
        UpdateRevenueProgressBarUI();
        UpdateTimeUI();
        GameManager.instance.AddMoney(info.GetCurrentRevenue());
        yield break;
    }
    //When there is a manager, he works for you
    IEnumerator GenerateIncome()
    {
        while(true)
        {
            //no need to update revenue progress bar
            if (info.state.revenueProgressBarTime < 0.25f)
            {
                //Aproximate revenue per second when time is too little
                BigNumber.StdBigNumber aprox = new BigNumber.StdBigNumber(info.GetCurrentRevenue());
                aprox *= (int)(1.0f / info.state.revenueProgressBarTime);
                GameManager.instance.AddMoney(aprox);
                revenueProgressBar.fillAmount = 1.0f;
                timeLeftText.text = "0";
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                while (currentTime < info.state.revenueProgressBarTime)
                {
                    currentTime += (Time.deltaTime + 0.05f);
                    UpdateRevenueProgressBarUI();
                    UpdateTimeUI();
                    yield return new WaitForSeconds(0.05f);
                }
                currentTime = 0.0f;
                UpdateRevenueProgressBarUI();
                UpdateTimeUI();
                GameManager.instance.AddMoney(info.GetCurrentRevenue());
            }
        }
    }
    public void Save()
    {
        GameManager.instance.onMoneyChanged -= InvestButtonChange;
        info.state.currentTime = currentTime;
        CapitalistTools.States.SaveBusinessState(info.state, info.name);
        info.state = null;
    }
    void OnApplicationQuit()
    {
        Save();
    }
    void OnApplicationFocus(bool focused)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return;
        if (!focused)
            Save();
    }
}
