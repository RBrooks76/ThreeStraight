using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viberation
{
    // Start is called before the first frame update
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject sysService;

    public void Vibrate()
    {
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        sysService = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }

    //Functions from https://developer.android.com/reference/android/os/Vibrator.html
    public void vibrate()
    {
        sysService.Call("vibrate");
    }
    public void vibrate(long milliseconds)
    {
        sysService.Call("vibrate", milliseconds);
    }
    public void vibrate(long[] pattern, int repeat)
    {
        sysService.Call("vibrate", pattern, repeat);
    }
    public void cancel()
    {
        sysService.Call("cancel");
    }
    public bool hasVibrator()
    {
        return sysService.Call<bool>("hasVibrator");
    }
}
