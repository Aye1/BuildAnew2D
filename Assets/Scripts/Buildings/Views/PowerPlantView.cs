using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections.Generic;

public class PowerPlantView : BuildingView
{
#pragma warning disable 0649
    [SerializeField] private List<Light2D> _lights;
#pragma warning restore 0649

    public override void SpecificUpdate()
    {
        foreach(Light2D light in _lights)
        {
            light.gameObject.SetActive(dataTile.IsOn);
        }
    }
}
