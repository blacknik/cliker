using UnityEngine;
using UnityEngine.UI;

public class UnlockUIElement : MonoBehaviour
{
    public Image businessIcon;
    public Text levelToUnlockText;
    public Text effect;

	public void SetupUI(BusinessInfo b, int unlockIndex)
    {
        businessIcon.sprite = b.icon;
        if(unlockIndex <= 0)
        {
            levelToUnlockText.text = "";
            effect.text = "Own this first";
        }
        else
        {
            levelToUnlockText.text = b.unlocks[unlockIndex].levelToUnlock.ToString();
            effect.text = "x" + b.unlocks[unlockIndex].timeMultiplier.ToString() + " on time";
        }
    }
}
