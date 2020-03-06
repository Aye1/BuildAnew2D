using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{

    public static ClickManager Instance { get; private set; }
    public SpriteRenderer selectedTileSprite;

    private BaseTileData _selectedTile;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ManageClick();
        }
        ManageSelectedTile();
    }

    private void ManageClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        BaseTileData tileClicked =  TilesDataManager.Instance.GetTileAtWorldPos(mouseWorldPos);
        if (tileClicked != null)
        {
            tileClicked.DebugOnClick();
            _selectedTile = tileClicked;
        }
        else
        {
            _selectedTile = null;
        }
    }

    private void ManageSelectedTile()
    {
        if(_selectedTile != null)
        {
            selectedTileSprite.transform.position = _selectedTile.worldPosition;
        }
        else
        {
            selectedTileSprite.transform.position = new Vector3(1000, 1000, 1000);
        }
    }
}
