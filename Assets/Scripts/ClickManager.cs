using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{

    public static ClickManager Instance { get; private set; }
    public SpriteRenderer selectedTileSprite;
    public BaseTileData SelectedTile { get; private set; }

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
            SelectedTile = tileClicked;
        }
        else
        {
            SelectedTile = null;
        }
    }

    private void ManageSelectedTile()
    {
        if(SelectedTile != null)
        {
            selectedTileSprite.transform.position = SelectedTile.worldPosition;
        }
        else
        {
            selectedTileSprite.transform.position = new Vector3(1000, 1000, 1000);
        }
    }
}
