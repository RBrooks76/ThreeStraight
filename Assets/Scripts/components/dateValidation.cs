using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dateValidation : Singleton<dateValidation>
{
    /// <summary>
    ///         Check whether date is valid or not.
    /// </summary>
    /// <param name="start">
    ///         start date
    /// </param>
    /// <param name="end">
    ///         end date
    /// </param>
    /// <returns>
    ///         true / false
    /// </returns>
    public bool CheckTournamentPeriod(string start, string end)
    {
        string[] s_arr = start.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        int startYear = int.Parse(s_arr[0]);
        int startMonth = int.Parse(s_arr[1]);
        int startDay = int.Parse(s_arr[2]);
        string[] e_arr = end.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        int endYear = int.Parse(e_arr[0]);
        int endMonth = int.Parse(e_arr[1]);
        int endDay = int.Parse(e_arr[2]);

        if (endYear < startYear)
        {
            return false;
        }
        else if (endYear == startYear && endMonth < startMonth)
        {
            return false;
        }
        else if (endYear == startYear && endMonth == startMonth && endDay < startDay)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    ///         Convert timestamp to Date.
    /// </summary>
    /// <param name="ts">
    ///         ts = timestamp
    /// </param>
    /// <returns>
    ///         Date in string.
    /// </returns>
    public string ConvertTimeStampToString(double ts)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts/1000).ToLocalTime();
        string formattedDate = dt.ToString("yyyy-MM-dd");
        return formattedDate;
    }

    /// <summary>
    ///         Calcuate Tournament Starting Time.
    /// </summary>
    /// <param name="ts">
    ///         ts = timestamp
    /// </param>
    /// <returns>
    ///         time in string.
    /// </returns>
    public string CalcTime(double ts)
    {
        string time = string.Empty;
        TimeSpan diffTime;
        DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts / 1000).ToLocalTime();
        DateTime currDate = DateTime.Now;
        if (startDate < currDate)
        {
            diffTime = currDate - startDate;
            time = FormatTime(diffTime, true);
        }
        else
        {
            diffTime =  startDate - currDate;
            time = FormatTime(diffTime, false);
        }
        return time;
    }

    private string FormatTime(TimeSpan ts, bool isStarted)
    {
        string display_time = string.Empty;
        int _day = ts.Days;
        int _hours = ts.Hours;
        int _minutes = ts.Minutes;
        if (!isStarted)
        {
            if (_day < 2)
            {
                if (_day == 0)
                    display_time = _hours + "H " + _minutes + " MIN";
                else
                    display_time = _day + "DAY " + _hours + "H " + _minutes + " MIN";
            }
            else
            {
                display_time = _day + "DAYS " + _hours + "H " + _minutes + " MIN";
            }
        }
        else
        {
            display_time = "PROGRESS";
        }
        return display_time;
    }
}
