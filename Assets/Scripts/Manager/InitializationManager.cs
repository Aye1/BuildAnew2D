using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public enum LoadingStrategy { LoadWithScene, LoadOnDemand }
public enum UnloadingStrategy { UnloadWithScene, NeverUnload }
public enum InitializationState { Unknown, Initializing, Updating, Ready }

[Serializable]
public class SceneManagersBinding
{
    public string sceneName;
    public List<ManagerLoadingStrategy> managers;
}

[Serializable]
public class ManagerLoadingStrategy
{
    public Manager manager;
    public LoadingStrategy loadingStrategy;
    public UnloadingStrategy unloadingStrategy;
}

public class InitializationManager : Manager
{
    public static InitializationManager Instance { get; private set; }

    private List<Manager> _instantiatedManagers;
    private int _currentManagersExpectedCount;

#pragma warning disable 0649
    [SerializeField] private List<SceneManagersBinding> _managers;
#pragma warning restore 0649

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitState = InitializationState.Initializing;
            _instantiatedManagers = new List<Manager>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManagersBinding binding = _managers.FirstOrDefault(x => x.sceneName == scene.name);
        InitializeManagers(binding);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SceneManagersBinding oldBinding = _managers.FirstOrDefault(x => x.sceneName == scene.name);
        DestroyManagers(oldBinding);
    }

    private void OnManagerStateChanged(Manager manager, InitializationState state)
    {
        if (_instantiatedManagers.All(x => x.InitState == InitializationState.Ready) && _instantiatedManagers.Count == _currentManagersExpectedCount)
        {
            InitState = InitializationState.Ready;
        }
    }

    private void InitializeManagers(SceneManagersBinding binding)
    {
        if (binding == default(SceneManagersBinding))
        {
            InitState = InitializationState.Unknown;
            throw new SceneNotFoundException();
        }
        else
        {
            InitState = InitializationState.Initializing;
            IEnumerable<Manager> managersToInit = binding.managers.Where(x => x.loadingStrategy == LoadingStrategy.LoadWithScene)
                                                                  .Select(x => x.manager);
            _currentManagersExpectedCount = managersToInit.Count();
            foreach (Manager manager in managersToInit)
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
            newManager.OnInitStateChanged += OnManagerStateChanged;
            newManager.transform.localPosition = Vector3.zero;
            _instantiatedManagers.Add(newManager);
        }
    }

    private void DestroyManagers(SceneManagersBinding binding)
    {
        if (binding == default(SceneManagersBinding))
        {
            InitState = InitializationState.Unknown;
            throw new SceneNotFoundException();
        }
        else
        {
            IEnumerable<Manager> managersToRemove = binding.managers.Where(x => x.unloadingStrategy == UnloadingStrategy.UnloadWithScene)
                                                                    .Select(x => x.manager)
                                                                    .Reverse();
            // The liste above is a list of prefabs, we want to remove the instantiated managers
            foreach (Manager manager in managersToRemove)
            {
                Manager instantiatedManager = _instantiatedManagers.FirstOrDefault(x => x.GetType() == manager.GetType());
                DestroyManager(instantiatedManager);
            }
        }
    }

    private void DestroyManager(Manager manager)
    {
        if (manager != null)
        {
            manager.OnInitStateChanged -= OnManagerStateChanged;
            _instantiatedManagers.Remove(manager);
            var instanceObject = manager.GetType().GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            instanceObject.SetValue(null, null);
            Destroy(manager.gameObject);
        }
    }

    public bool ManagerExists(Manager manager)
    {
        if (manager == null)
        {
            Debug.LogWarning("Null manager provided");
            return false;
        }
        return _instantiatedManagers.Any(x => x.GetType() == manager.GetType());
    }

    public void AskForManagerCreation(Type type)
    {
        SceneManagersBinding binding = _managers.FirstOrDefault(x => x.sceneName == SceneManager.GetActiveScene().name);
        if (binding == default(SceneManagersBinding))
        {
            InitState = InitializationState.Unknown;
            throw new SceneNotFoundException();
        }
        else
        {
            InitState = InitializationState.Updating;
            Manager manager = binding.managers.FirstOrDefault(x => x.manager.GetType() == type).manager;
            if(manager == default(Manager))
            {
                throw new ManagerNotFoundInSceneException();
            }
            else
            {
                _currentManagersExpectedCount++;
                InitalizeManager(manager);
            }
        }
    }

    // This manager should never be destroyed, but just in case
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
