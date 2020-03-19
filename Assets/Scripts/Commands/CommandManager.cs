using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance { get; private set; }

    private Stack<Command> _commandsQueue;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            _commandsQueue = new Stack<Command>();
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void ExecuteCommand(Command cmd)
    {
        _commandsQueue.Push(cmd);
        cmd.Execute();
    }

    public void UndoLastCommand()
    {
        if (_commandsQueue.Count > 0)
        {
            Command cmd = _commandsQueue.Pop();
            cmd.Undo();
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
}
