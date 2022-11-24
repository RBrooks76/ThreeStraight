using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inputfield : MonoBehaviour, IPointerClickHandler
{
    InputField inputField;
    TouchScreenKeyboard keyboard;
    public Transform container;
    bool keyboardOpen = false;
    float keyboardHeight = -1;
    string keyHeight = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        inputField = transform.GetComponent<InputField>();
    }

    /**
     * 
     *  InputField pointer Event (Display keyboard)
     * 
     * **/
    public void OnPointerClick(PointerEventData eventData)
    {
        if (keyboard == null)
        {
            keyboard = TouchScreenKeyboard.Open(string.Empty);
            TouchScreenKeyboard.hideInput = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((Application.platform == RuntimePlatform.Android) ||
        (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            if (keyboard.active && !keyboardOpen)
            {
                StartCoroutine(GetKeyboardHeight());
                keyboardOpen = true;
            }
        }
    }
    IEnumerator GetKeyboardHeight()
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
#if UNITY_ANDROID
            using (AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
                using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);
                    keyboardHeight = Screen.height - Rct.Call<int>("height");
                }
            }
#elif UNITY_IOS
        keyboardHeight = (int)TouchScreenKeyboard.area.height;
#else
        keyboardHeight = 0;
#endif
            if (keyboardHeight < 50)
            {
                keyboardOpen = false;
                //container.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f, 0.0f);
                container.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
            //container.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, keyboardHeight, 0.0f);
            container.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, keyboardHeight, 0.0f);
        } while (keyboardOpen);
    }
}
