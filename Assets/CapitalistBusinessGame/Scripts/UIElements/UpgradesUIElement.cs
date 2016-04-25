using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradesUIElement : MonoBehaviour
{
    public Button buyButton;
    public Text costText;
    public Text effect;
    public Image icon;

    private Upgrade info;

    public void ButtonsChange()
    {
        //if upgrades window is not active then no need to update buttons
        if (GameManager.instance.ui.upgradesWindow.activeSelf == false)
            return;
        if (GameManager.instance.HasEnoughMoney(info.cost))
            buyButton.interactable = true;
        else
            buyButton.interactable = false;
    }
    public void SetupUI(Upgrade u)
    {
        info = u;
        icon.sprite = info.appliesToBusiness.icon;
        effect.text = "x" + info.revenueMultiplier.ToString() + " Revenue";
        costText.text = CapitalistTools.MoneyTools.GetFormatedMoneyText(info.cost) + " " + CapitalistTools.MoneyTools.GetScaleName(info.cost);
        buyButton.onClick.AddListener(new UnityAction(ApplyUpgrade));
        GameManager.instance.onMoneyChanged += ButtonsChange;
    }
    public void ApplyUpgrade()
    {
        if(GameManager.instance.HasEnoughMoney(info.cost) && info.state.isUnlocked == false && info.appliesToBusiness.state.isOwned)
        {
            info.state.isUnlocked = true;

            //Increase revenue multiplier
            info.appliesToBusiness.state.currentRevenueMultiplier += info.revenueMultiplier;

            GameManager.instance.onMoneyChanged -= ButtonsChange;
            GameManager.instance.LoseMoney(info.cost);
            CapitalistTools.States.SaveUpgradeState(info.state, info.name);

            GameManager.instance.audioManager.PlaySound(SoundType.UPGRADE);
            GameManager.instance.ui.UpdateBusinessRevText();
            transform.SetParent(null, false);

            Destroy(gameObject);
        }
    }
}
