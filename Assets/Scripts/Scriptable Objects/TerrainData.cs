using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TerrainData", order = 1)]
public class TerrainData : ScriptableObject
{
    public bool canBeFlooded = true;
}
