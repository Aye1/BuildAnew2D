using System.Collections.Generic;
using UnityEngine;

public class CommandManager : Manager
{
    public static CommandManager Instance { get; private set; }

    private Stack<Command> _commandsQueue;

    #region Events
    public delegate void CommandExecution(Command cmd);
    public static CommandExecution OnCommandDone;
    public static CommandExecution OnCommandUndone;
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            _commandsQueue = new Stack<Command>();
            RegisterCallbacks();
            InitState = InitializationState.Ready;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void ExecuteCommand(Command cmd)
    {
        if (cmd.isUndoable)
        {
            _commandsQueue.Push(cmd);
        }
        cmd.Execute();
        Debug.Log("Executing command - " + cmd.GetDescription() + (cmd.isUndoable ? "" : " (not undoable)"));
        OnCommandDone?.Invoke(cmd);
    }

    public void UndoLastCommand()
    {
        if (_commandsQueue.Count > 0)
        {
            Command cmd = _commandsQueue.Pop();
            cmd.Undo();
            Debug.Log("Undoing command - " + cmd.GetDescription());
            OnCommandUndone?.Invoke(cmd);
        }
    }

    public bool CanUndoLastCommand()
    {
        return _commandsQueue.Count > 0;
    }

    public void EmptyCommandList()
    {
        _commandsQueue.Clear();
    }

    private void RegisterCallbacks()
    {
        TurnManager.OnTurnStart += EmptyCommandList;
    }

    private void UnregisterCallbacks()
    {
        TurnManager.OnTurnStart -= EmptyCommandList;
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }
}
