using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections.Generic;

public class PowerPlant : Building
{
    [SerializeField] private List<Light2D> _lights;

    public override void SpecificUpdate()
    {
        foreach(Light2D light in _lights)
        {
            light.gameObject.SetActive(dataTile.IsOn);
        }
    }
}
