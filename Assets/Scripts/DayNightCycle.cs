using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Animation _dayNightAnimation;
    // Start is called before the first frame update
    void Start()
    {
        _dayNightAnimation = GetComponent<Animation>();
        TurnManager.OnTurnStart += OnTurnStarted;        
    }

    public void OnTurnStarted()
    {
        UIManager.Instance.RequestBlockUI(_dayNightAnimation.clip.length);
        gameObject.GetComponent<Animation>().Play();
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStart -= OnTurnStarted;
    }

}
