using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayerStats : MonoBehaviour
{
    [SerializeField] public BaseStats playerStats;
    public void AddSOStats()
    {
        GameManager.Instance.AddPlayerStats(playerStats);
    }
}
