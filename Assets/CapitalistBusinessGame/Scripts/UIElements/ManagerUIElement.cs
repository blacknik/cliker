using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ManagerUIElement : MonoBehaviour
{
    private Manager manager;

    [Header("UI Stuff")]
    public Image managerIcon;
    public Image businessIcon;
    public Text moneyText;
    public Text description;
    public Button hireButton;

    public void ChangeButtons()
    {
        if (GameManager.instance.ui.managerWindow.activeSelf == false)
            return;
        if (GameManager.instance.HasEnoughMoney(manager.costsToHire))
            hireButton.interactable = true;
        else
            hireButton.interactable = false;
    }
    public void SetupUI(Manager m)
    {
        manager = m;
        managerIcon.sprite = manager.icon;
        businessIcon.sprite = manager.runsBusiness.icon;
        moneyText.text = CapitalistTools.MoneyTools.GetFormatedMoneyText(new BigNumber.StdBigNumber(manager.costsToHire)) + " " + CapitalistTools.MoneyTools.GetScaleName(manager.costsToHire);
        description.text = "Runs " + manager.runsBusiness.name;
        hireButton.onClick.AddListener(new UnityAction(this.Hire));
        GameManager.instance.onMoneyChanged += ChangeButtons;
    }
    void Hire()
    {
        if (manager.runsBusiness.state.isOwned == false)
            return;
        if (!GameManager.instance.HasEnoughMoney(manager.costsToHire))
            return;
        GameManager.instance.onMoneyChanged -= ChangeButtons;           //No longer listen for money change
        GameManager.instance.LoseMoney(manager.costsToHire);
        manager.state.isHired = true;
        CapitalistTools.States.SaveManagerState(manager.state, manager.name);
        GameManager.instance.audioManager.PlaySound(SoundType.UPGRADE);

        manager.runsBusiness.state.hasManager = true;
        transform.SetParent(null, false);
        Destroy(gameObject);
    }
}
