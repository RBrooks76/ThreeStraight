using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace hyhy.Common
{
    public class UnityCalendar : MonoBehaviour
    {
        [SerializeField] private ExpandedDropdown dropdownYear;
        [SerializeField] private ExpandedDropdown dropdownMonth;
        [SerializeField] private ExpandedDropdown dropdownDay;
        private List<int> yearGroup = new List<int>();
        private List<int> monthGroup = new List<int>();
        private List<int> dayGroup = new List<int>();
        private int curYearValue = -1;
        private int curMonthValue = -1;
        private int curDayValue = -1;

        [SerializeField] private int curYear = -1;
        [SerializeField] private int curMonth = -1;
        [SerializeField] private int curDay = -1;
        DateTime CurDateNow;

        private void Awake()
        {
            CurDateNow = DateTime.Now;
            dropdownYear.onValueChanged.AddListener(SetYear);
            dropdownMonth.onValueChanged.AddListener(SetMonth);
            dropdownDay.onValueChanged.AddListener(SetDay);

            Init();
        }
        public void Init()
        {
            curYear = curMonth = curDay = -1;
            dropdownYear.value = 0;
            dropdownMonth.value = 0;
            dropdownDay.value = 0;
            UpdateYear();
        }

        public DateTime GetDate()
        {
            DateTime res = DateTime.Now;
            try
            {
                res = new DateTime(curYear, curMonth, curDay);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return res;
        }

        public void SetYear(int value)
        {
            curYearValue = value - 1;
            curYear = curYearValue >= 0 ? yearGroup[curYearValue] : -1;
            UpdateMonth();
            UpdateDay();
        }

        public void SetMonth(int value)
        {
            curMonthValue = value - 1;
            curMonth = curMonthValue >= 0 ? monthGroup[curMonthValue] : -1;
            UpdateDay();
        }

        public void SetDay(int value)
        {
            curDayValue = value - 1;
            curDay = curDayValue >= 0 ? dayGroup[curDayValue] : -1;
        }

        private void UpdateYear()
        {
            int curYear = CurDateNow.Year;
            List<string> op = new List<string>();
            int curDate = dropdownMonth.value;
            yearGroup = new List<int>();
            op.Add("Year");
            dropdownYear.ClearOptions();
            for (int i = curYear; i >= 1900; i--)
            {
                yearGroup.Add(i);
                op.Add(i.ToString());

            }
            dropdownYear.AddOptions(op);

            curDate = Mathf.Clamp(curDate, 0, yearGroup.Count);
            dropdownYear.value = curDate;

            UpdateMonth();
            UpdateDay();
        }

        private void UpdateMonth()
        {
            int year = curYearValue >= 0 ? yearGroup[curYearValue] : CurDateNow.Year;
            DateTime dateTime = new DateTime(year, 1, 1);
            DateTime firstMonth = dateTime.AddMonths(-(dateTime.Month - 1));
            DateTime thatMonth;
            int date = 0;
            int curDate = dropdownMonth.value;
            List<string> op = new List<string>();
            monthGroup = new List<int>();
            op.Add("Month");
            dropdownMonth.ClearOptions();

            while (true)
            {
                thatMonth = firstMonth.AddMonths(date);
                if (thatMonth.Year == firstMonth.Year)
                {
                    date++;
                    monthGroup.Add(date);
                    op.Add(date.ToString());
                }
                else
                    break;
            }


            dropdownMonth.AddOptions(op);

            curDate = Mathf.Clamp(curDate, 0, monthGroup.Count);
            dropdownMonth.value = curDate;
        }

        private void UpdateDay()
        {
            int year = curYearValue >= 0 ? yearGroup[curYearValue] : CurDateNow.Year;
            int month = curMonthValue >= 0 ? monthGroup[curMonthValue] : CurDateNow.Month;
            DateTime dateTime = new DateTime(year, month, 1);
            DateTime firstDay = dateTime.AddDays(-(dateTime.Day - 1));
            DateTime thatDay;

            int date = 0;
            int curDate = dropdownDay.value;

            List<string> op = new List<string>();
            dayGroup = new List<int>();
            op.Add("Day");
            dropdownDay.ClearOptions();

            while (true)
            {
                thatDay = firstDay.AddDays(date);
                if (thatDay.Month == firstDay.Month)
                {
                    date++;
                    dayGroup.Add(date);
                    op.Add(date.ToString());
                }
                else
                    break;
            }

            dropdownDay.AddOptions(op);
            curDate = Mathf.Clamp(curDate, 0, dayGroup.Count);
            dropdownDay.value = curDate;
        }
    }
}
