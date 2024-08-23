using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResource
{
    private string name;
    private int currentAmount;

    public GameResource(string name, int initialAmount)
    {
        this.name = name;
        this.currentAmount = initialAmount;
    }

    public void AddAmount(int amount)
    {
        currentAmount += amount;
        if (currentAmount < 0) currentAmount = 0;
    }

    public string Name { get => name; }
    public int CurrentAmount { get => currentAmount; }
}
