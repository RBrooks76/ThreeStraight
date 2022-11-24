using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class optionAlert : MonoBehaviour
{
    public Transform optionMain;
    public Transform soundArea;
    public Transform musicArea;
    public Transform viberationArea;
    public Transform contactusArea;
    public GameObject soundFxBtn;
    public GameObject musicBtn;
    public GameObject viberationBtn;
    public GameObject contactusBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        UI_Main.UIM.isChangeSetting = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (UI_Main.UIM.isChangeSetting)
        {
            UI_Main.UIM.isChangeSetting = false;
            changeSettingStatus();
        }
    }
    private void changeSettingStatus()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("ThreeStraight_Sound")))
        {
            soundFxBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
        }
        else
        {
            if (PlayerPrefs.GetString("ThreeStraight_Sound") == gamePropertySettings.OPTIONS_SOUNDFX_TRUE)
            {
                soundFxBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
            }
            else
            {
                soundFxBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
            }
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("ThreeStraight_Music")))
        {
            musicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
        }
        else
        {
            if (PlayerPrefs.GetString("ThreeStraight_Music") == gamePropertySettings.OPTIONS_MUSIC_TRUE)
            {
                musicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
            }
            else
            {
                musicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
            }
        }

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("ThreeStraight_Vibration")))
        {
            viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
        }
        else
        {
            if (PlayerPrefs.GetString("ThreeStraight_Vibration") == gamePropertySettings.OPTIONS_VIBERATION_TRUE)
            {
                viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
            }
            else
            {
                viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
            }
        }
    }
    public void onChnageSound()
    {
        if (!UI_Main.UIM.soundPlayer.mute)
        {
            UI_Main.UIM.soundPlayer.mute = true;
            soundFxBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
            PlayerPrefs.SetString("ThreeStraight_Sound", gamePropertySettings.OPTIONS_SOUNDFX_TRUE);
            PlayerPrefs.Save();
        }
        else
        {
            UI_Main.UIM.soundPlayer.mute = false;
            soundFxBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
            PlayerPrefs.SetString("ThreeStraight_Sound", gamePropertySettings.OPTIONS_SOUNDFX_FALSE);
            PlayerPrefs.Save();
        }
        UI_Main.UIM.isChangeSetting = true;
    }
    public void onChnageMusic()
    {
        if (!UI_Main.UIM.musicPlayer.mute)
        {
            UI_Main.UIM.musicPlayer.mute = true;
            musicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
            PlayerPrefs.SetString("ThreeStraight_Music", gamePropertySettings.OPTIONS_MUSIC_TRUE);
            PlayerPrefs.Save();
        }
        else
        {
            UI_Main.UIM.musicPlayer.mute = false;
            UI_Main.UIM.musicPlayer.loop = true;
            musicBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
            PlayerPrefs.SetString("ThreeStraight_Music", gamePropertySettings.OPTIONS_MUSIC_FALSE);
            PlayerPrefs.Save();
        }
        UI_Main.UIM.isChangeSetting = true;
    }
    public void onChangeViberation()
    {
        if (!UI_Main.UIM.isVibration)
        {
            UI_Main.UIM.isVibration = true;
            viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_on");
            PlayerPrefs.SetString("ThreeStraight_Vibration", gamePropertySettings.OPTIONS_MUSIC_TRUE);
            PlayerPrefs.Save();
        }
        else
        {
            UI_Main.UIM.isVibration = false;
            viberationBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("designs/switch_off");
            PlayerPrefs.SetString("ThreeStraight_Vibration", gamePropertySettings.OPTIONS_MUSIC_FALSE);
            PlayerPrefs.Save();
        }
        UI_Main.UIM.isChangeSetting = true;
    }
    public void ContactUs()
    {
        UI_Main.UIM.CleanInputField();
        UI_Main.UIM.contactUsPage.gameObject.SetActive(true);
    }
}
