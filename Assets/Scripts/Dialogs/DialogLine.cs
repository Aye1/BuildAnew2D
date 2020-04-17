public class DialogLine
{
    public Character character;
    public string text;

    public DialogLine(string text, Character character)
    {
        this.text = text;
        this.character = character;
    }

    public DialogLine(string text) : this(text, null)
    {
    }
}