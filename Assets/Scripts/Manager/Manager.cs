using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    public delegate void InitStateChanged(Manager sender, InitializationState newState);
    public static InitStateChanged OnInitStateChanged;
    
    private InitializationState _initState;
    public InitializationState InitState
    {
        get { return _initState; }
        set
        {
            if(value != _initState)
            {
                _initState = value;
                Debug.Log(name + " passed to state " + _initState);
                OnInitStateChanged?.Invoke(this, _initState);
            }
        }
    }
}
