using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource source;

    [Header("UI")]
    public Image audioImage;
    public Sprite audioOn;
    public Sprite audioOff;

    [Header("Audio Sounds")]
    public AudioClip investSound;
    public AudioClip unlockSound;
    public AudioClip upgradeSound;
    public AudioClip clickSound;

    void Start()
    {
        source = GetComponent<AudioSource>();
        int muted = PlayerPrefs.GetInt("Muted");
        Change(muted);
    }
    public void PlaySound(SoundType sound)
    {
        if (source == null)
            return;
        source.Stop();
        switch(sound)
        {
            case SoundType.INVEST:
                source.PlayOneShot(investSound);
                break;
            case SoundType.UNLOCK:
                source.PlayOneShot(unlockSound);
                break;
            case SoundType.UPGRADE:
                source.PlayOneShot(upgradeSound);
                break;
            case SoundType.CLICK:
                source.PlayOneShot(clickSound);
                break;
        }
    }
    public void Mute()
    {
        int muted = PlayerPrefs.GetInt("Muted");
        muted = (muted == 0) ? 1 : 0;
        Change(muted);
        PlayerPrefs.SetInt("Muted", muted);
    }
    void Change(int muted)
    {
        source.mute = (muted == 0) ? false : true;
        audioImage.sprite = (muted == 0) ? audioOn : audioOff;
    }
}
public enum SoundType
{
    INVEST,
    UNLOCK,
    UPGRADE,
    CLICK
}