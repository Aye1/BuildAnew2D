
using UnityEngine;


public class DayNightCycle : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        TurnManager.OnTurnStart += OnTurnStarted;        
    }

    public void OnTurnStarted()
    {
        gameObject.GetComponent<Animation>().Play();
    }
   
}
