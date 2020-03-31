using System.Collections.Generic;

public abstract class Command
{
    public abstract void Execute();
    public abstract void Undo();
    public abstract string GetDescription();
}
