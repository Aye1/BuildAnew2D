using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }
    public int woodAmount = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void AddWood(int amount)
    {
        if (amount > 0)
        {
            woodAmount += amount;
        }
    }

    public void RemoveWood(int amount)
    {
        if(woodAmount - amount >= 0)
        {
            woodAmount = Mathf.Max(woodAmount - amount, 0);
        }
    }
}
