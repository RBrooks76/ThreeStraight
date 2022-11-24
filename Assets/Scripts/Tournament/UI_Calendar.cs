using hyhy.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Calendar : MonoBehaviour
{
    [SerializeField] private UnityCalendar unityCalendar;
    private string text;


    public void OnClickConfirm()
    {
        DateTime dt = unityCalendar.GetDate();
        text = dt.ToString("yyyy-MM-dd");
        if (UI_Ranking.UIR.startDateSelected)
        {
            UI_Ranking.UIR.startDate = text;
        }
        else if (UI_Ranking.UIR.endDateSelected)
        {
            UI_Ranking.UIR.endDate = text;
        }

        if (!string.IsNullOrEmpty(text))
        {
            CleanDate();
        }
    }


    public void OnClickClose()
    {
        unityCalendar.Init();
        CleanDate();
    }


    private void CleanDate()
    {
        text = string.Empty;
        transform.gameObject.SetActive(false);
    }
}
