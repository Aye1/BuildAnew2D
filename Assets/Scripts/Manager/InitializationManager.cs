using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

[Serializable]
public class SceneManagersBinding
{
    public string sceneName;
    public List<Manager> managers;
}

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance { get; private set; }

#pragma warning disable 0649
    [SerializeField] private List<SceneManagersBinding> _managers;
#pragma warning restore 0649

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChange;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneChange(Scene current, Scene next)
    {
        Debug.Log("OnSceneChanged");
        SceneManagersBinding binding = _managers.FirstOrDefault(x => x.sceneName == next.name);
        if(binding == default(SceneManagersBinding))
        {
            throw new SceneNotFoundException();
        }
        else
        {
            foreach(Manager manager in binding.managers)
            {
                InitalizeManager(manager);
            }
        }
    }

    private void InitalizeManager(Manager manager)
    {
        if(ManagerExists(manager))
        {
            Debug.Log("Manager of type " + manager.GetType() + " already created");
            return;
        } else
        {
            Manager newManager = Instantiate(manager, Vector3.zero, Quaternion.identity, transform);
            newManager.transform.localPosition = Vector3.zero;
        }
    }

    public bool ManagerExists(Manager manager)
    {
        if (manager == null)
        {
            Debug.LogWarning("Null manager provided");
            return false;
        }
        var instanceObject = manager.GetType().GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        if(instanceObject == null)
        {
            Debug.LogError("Instance not found on type " + manager.GetType());
            throw new ReflectionPropertyNotFoundException();
        }
        return instanceObject.GetValue(null) != null;
    }
}
