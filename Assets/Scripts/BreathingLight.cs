using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class BreathingLight : MonoBehaviour
{
    private Light2D _light;

    public float maxIntensity;
    public float minIntensity;
    public float cycleDuration;
    public AnimationCurve fullCycle;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _light.intensity = GetIntensityOnCurve(GetCurveTime());
    }

    private float GetCurveTime()
    {
        return (Time.time % cycleDuration) / cycleDuration;
    }

    private float GetIntensityOnCurve(float time)
    {
        float baseIntensity = fullCycle.Evaluate(time);
        return baseIntensity * maxIntensity + (1-baseIntensity) * minIntensity;
    }
}
