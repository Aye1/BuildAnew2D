using System.Collections.Generic;

public abstract class ActiveTile
{
    protected ActiveTile() => Init();

    public virtual void Init() 
    {
        OnTileModified?.Invoke(); 
    }

    public virtual void OnTurnStarts(IEnumerable<BaseTileData> neighbours) { }
    public virtual void PredictOnTurnStarts(IEnumerable<BaseTileData> neighbours) { }
    public virtual void ApplyPrediction() { } 

    public virtual string GetDebugText() { return ""; }
    public virtual string GetText() { return ""; }

    public virtual void DebugOnClick() { }
    private bool _isSelected = false;
    public bool IsSelected()
    {
        return _isSelected;
    }
    public void SetIsSelected(bool isSelected)
    {
        _isSelected = isSelected;
        InternalSelection();
    }
    public virtual void InternalSelection() { }

    #region Events

    public delegate void PropertyChanged(string propertyName);
    public event PropertyChanged OnPropertyChanged;

    public delegate void TileModified();
    public static event TileModified OnTileModified;

    protected virtual void OnSpecificTileModified()
    {
        OnTileModified?.Invoke();
    }

    protected virtual void OnSpecificPropertyChanged(string propertyName)
    {
        OnPropertyChanged?.Invoke(propertyName);
    }

    #endregion


}
