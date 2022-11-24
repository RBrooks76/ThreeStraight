using System.Collections.Generic;
using UnityEngine;

public class ThreeStraight : Singleton<ThreeStraight>
{
    private int _maxIdx;

    public Dictionary<int, List<int>> winCombos = new Dictionary<int, List<int>>();
    public Dictionary<int, List<int>> movablePos = new Dictionary<int, List<int>>();
    public int idx = 0;
    private int movableInx = 0;

    public void Init()
    {
        List<int> successList = new List<int>();
        successList.Add(0);
        successList.Add(1);
        successList.Add(2);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(3);
        successList.Add(4);
        successList.Add(5);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(6);
        successList.Add(7);
        successList.Add(8);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(0);
        successList.Add(3);
        successList.Add(6);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(1);
        successList.Add(4);
        successList.Add(7);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(2);
        successList.Add(5);
        successList.Add(8);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(0);
        successList.Add(4);
        successList.Add(8);
        AddDictionary(successList);
        successList = new List<int>();
        successList.Add(6);
        successList.Add(4);
        successList.Add(2);
        AddDictionary(successList);

        List<int> movableList = new List<int>();
        movableList.Add(1);
        movableList.Add(3);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(0);
        movableList.Add(2);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(1);
        movableList.Add(5);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(0);
        movableList.Add(6);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(0);
        movableList.Add(1);
        movableList.Add(2);
        movableList.Add(3);
        movableList.Add(5);
        movableList.Add(6);
        movableList.Add(7);
        movableList.Add(8);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(2);
        movableList.Add(8);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(3);
        movableList.Add(7);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(6);
        movableList.Add(8);
        movableList.Add(4);
        AddMovableDic(movableList);
        movableList = new List<int>();
        movableList.Add(5);
        movableList.Add(7);
        movableList.Add(4);
        AddMovableDic(movableList);
    }
    private void AddDictionary(List<int> list)
    {
        winCombos.Add(idx, list);
        idx++;
    }
    public List<int> GetListFromDictionary(int index)
    {
        return winCombos[index];
    }
    public int WinCombosCount()
    {
        return winCombos.Count;
    }
    private void AddMovableDic(List<int> list)
    {
        movablePos.Add(movableInx, list);
        movableInx++;
    }
    public List<int> GetMovableListFromDictionary(int index)
    {
        return movablePos[index];
    }

}
