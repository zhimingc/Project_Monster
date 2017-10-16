using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

  public List<List<GameObject>> grid;
  public GameObject gridObj;
  public Vector2 gridBlockSize;
  public int gridSizeX, gridSizeY;
  public float spacing;

  // Use this for initialization
  void Awake () {
    ConstructLevel();
  }

  void Start()
  {
    // init depending on starting sequence
    GameStateSettings();
  }

  void GameStateSettings()
  {
    switch (GameManager.Instance.gameState)
    {
      case GAME_STATE.TUTORIAL:
        TutorialGridStates(0);
        break;
      case GAME_STATE.START_SEQUENCE:

        break;
    }
  }

  void Update()
  {

  }

  GameObject GetGrid(int x, int y)
  {
    return grid[x][y];
  }

  public void ToggleGrid(bool flag)
  {
    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        grid[x][y].SetActive(flag);
      }
    }
  }

  // reset board; calls ResetHoveredGrid()
  public void ResetGrid()
  {
    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        grid[x][y].GetComponent<GridScript>().ResetHoveredGrid();
      }
    }
  }

  // checks if the grid is full
  public bool IfGridFull()
  {
    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        if (grid[x][y].GetComponent<GridScript>().IsGridFull())
        {
          return true;
        }
      }
    }

    // no grids are full
    return false;
  }

  public bool CheckIfLegalMove(GridScript singleGrid, BlockBehaviour block)
  {
    Vector2[] layout = block.layout;
    Vector2 gridCoordinates = new Vector2(singleGrid.coordinates[0], singleGrid.coordinates[1]);
    bool canFit = true; 

    for (int i = 0; i < layout.Length; ++i)
    {
      // checking bounds
      Vector2 destGrid = gridCoordinates + layout[i];

      // Outside of grid
      if (destGrid.x < 0.0f || destGrid.x >= gridSizeX ||
        destGrid.y < 0.0f || destGrid.y >= gridSizeY)
      {
        canFit = false;
      }
      else
      {
        canFit = true;
        block.isReverseLayout = false;
        break;
      }

      // check reverse layout
      destGrid = gridCoordinates - layout[i];
      if (destGrid.x < 0.0f || destGrid.x >= gridSizeX ||
        destGrid.y < 0.0f || destGrid.y >= gridSizeY)
      {
        canFit = false;
      }
      else
      {
        canFit = true;
        block.isReverseLayout = true;
        break;
      }
    }

    return canFit;
  }

  public void AddBlockToGrid(GridScript singleGrid, BlockBehaviour block)
  {
    Vector2[] layout = block.layout;
    Vector2 gridCoordinates = new Vector2(singleGrid.coordinates[0], singleGrid.coordinates[1]);
    //singleGrid.AddToStack(block.ingredients[0].GetComponent<IngredientScript>());
    singleGrid.AddToStack(0);

    // Plus 1 because index 0 is the center
    for (int i = 0; i < layout.Length; ++i)
    {
      Vector2 layoutDir = block.isReverseLayout ? -layout[i] : layout[i];
      Vector2 destGrid = gridCoordinates + layoutDir;

      // Call add ingredient
      GridScript curGrid = grid[(int)destGrid.x][(int)destGrid.y].GetComponent<GridScript>();
      curGrid.AddToStack(i + 1);
      //curGrid.AddToStack(block.ingredients[i + 1].GetComponent<IngredientScript>());
    }
  }


  public void RemoveBlockFromGrid(GridScript singleGrid, BlockBehaviour block)
  {
    Vector2[] layout = block.layout;
    Vector2 gridCoordinates = new Vector2(singleGrid.coordinates[0], singleGrid.coordinates[1]);
    singleGrid.RemoveFromStack(0);

    // Plus 1 because index 0 is the center
    for (int i = 0; i < layout.Length; ++i)
    {
      Vector2 layoutDir = block.isReverseLayout ? -layout[i] : layout[i];
      Vector2 destGrid = gridCoordinates + layoutDir;

      // Call add ingredient
      GridScript curGrid = grid[(int)destGrid.x][(int)destGrid.y].GetComponent<GridScript>();
      curGrid.RemoveFromStack(i+1);
    }
  }

  public void ConstructLevel()
  {
    //ConstructGrid(gridSizeX, gridSizeY);

    // Get level by reference
    grid = new List<List<GameObject>>();

    for (int x = 0; x < 2; ++x)
    {
      grid.Add(new List<GameObject>());
      for (int y = 0; y < 2; ++y)
      {
        string gridToFind = "grid_" + x.ToString() + "x" + y.ToString();
        grid[x].Add(GameObject.Find(gridToFind));
      }
    }
  }

  public void ConstructGrid(int[] gridSizes)
  {
    //ConstructGrid(gridSizes[0], gridSizes[1]);
  }

  public void ToggleDinnerShiftGrids(bool flag)
  {
    GRID_TYPE type = flag ? GRID_TYPE.BOWL : GRID_TYPE.PLATE;

    grid[0][0].GetComponent<GridScript>().ToggleGridType(type);
    grid[0][0].GetComponentInChildren<GridScript>().EmitEatenParticles();
    GameFeel.ShakeCameraRandom(new Vector3(0.05f, 0.05f, 0.0f), new Vector3(-0.05f, -0.05f, 0.0f), 4, 0.2f);
    // play sfx
    GameManager.Instance.SFX().PlaySoundWithPitch("boom1", 0.9f, 1.1f);
    // Update monster requests
    GameManager.Instance.monsterMan.UpdateMonsterRequests();

    LeanTween.delayedCall(0.5f, () =>
    {
      grid[1][1].GetComponent<GridScript>().ToggleGridType(type);
      grid[1][1].GetComponentInChildren<GridScript>().EmitEatenParticles();
      GameFeel.ShakeCameraRandom(new Vector3(0.05f, 0.05f, 0.0f), new Vector3(-0.05f, -0.05f, 0.0f), 4, 0.2f);
      // play sfx
      GameManager.Instance.SFX().PlaySoundWithPitch("boom1", 0.9f, 1.1f);
      // Update monster requests
      GameManager.Instance.monsterMan.UpdateMonsterRequests();
    });


  }

  void ConstructGrid(int gridX, int gridY)
  {
    int halfGridX = gridX / 2;
    int halfGridY = gridY / 2;
    int xOffset = gridSizeX % 2;
    int yOffset = gridSizeY % 2;
    float posOffsetX = xOffset == 0 ? gridBlockSize.x / 2.0f : 0;
    float posOffsetY = yOffset == 0 ? gridBlockSize.y / 2.0f : 0;
    grid = new List<List<GameObject>>();

    for (int x = -halfGridX; x < halfGridX + xOffset; ++x)
    {
      grid.Add(new List<GameObject>());
      for (int y = -halfGridY; y < halfGridY + yOffset; ++y)
      {
        Vector3 instPos = new Vector3(x * (gridBlockSize.x + spacing) + posOffsetX, 
          y * (gridBlockSize.y + spacing) + posOffsetY, 0.0f);
        instPos += transform.position;  // Add position of obj to offset entire grid

        GameObject curGridBlock = Instantiate(gridObj, instPos, Quaternion.identity);
        curGridBlock.transform.localScale = new Vector3(gridBlockSize.x, gridBlockSize.y, 1.0f);

        // Set coordinates of grid
        GridScript gs = curGridBlock.GetComponent<GridScript>();
        gs.coordinates = new int[] { x + halfGridX, y + halfGridY };
        grid[x + halfGridX].Add(curGridBlock);
      }
    }
  }

  public void ApplyTurntable(bool direction)
  {
    // applies turntable
    if (direction)
    {
      GetGrid(0, 0).GetComponent<GridScript>().MoveStackTo(GetGrid(0, 1));
      GetGrid(0, 1).GetComponent<GridScript>().MoveStackTo(GetGrid(1, 1));
      GetGrid(1, 0).GetComponent<GridScript>().MoveStackTo(GetGrid(0, 0));
      GetGrid(1, 1).GetComponent<GridScript>().MoveStackTo(GetGrid(1, 0));
    }
    else
    {
      // reverts turntable
      GetGrid(0, 0).GetComponent<GridScript>().MoveStackBack();
      GetGrid(0, 1).GetComponent<GridScript>().MoveStackBack();
      GetGrid(1, 1).GetComponent<GridScript>().MoveStackBack();
      GetGrid(1, 0).GetComponent<GridScript>().MoveStackBack();
    }
  }

  public void MonsterAffectGrid(Request monReq)
  {
    bool applied = false;
    while (applied == false)
    {
      // randomly select a grid to affect
      int gridIndex = Random.Range(0, 4);
      int colIndex = 0;
      // call effect on grid
      if (gridIndex > 1)
      {
        colIndex = 1;
        gridIndex -= 2;
      }
      applied = grid[colIndex][gridIndex].GetComponent<GridScript>().MonsterEffect(monReq);
    }
  }

  public bool AnyGridsServable()
  {
    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        if (grid[x][y].GetComponent<GridScript>().canServe) return true;
      }
    }

    return false;
  }

  public void TutorialGridStates(int stage)
  {
    switch (stage)
    {
      case 0:
        grid[0][1].SetActive(false);
        grid[1][1].SetActive(false);
        grid[1][0].SetActive(false);
        break;
      case 1:
        grid[0][1].SetActive(true);
        grid[1][1].SetActive(true);
        grid[1][0].SetActive(true);

        List<INGREDIENT_TYPE> ing = new List<INGREDIENT_TYPE>();
        ing.Add(INGREDIENT_TYPE.BREAD);
        ing.Add(INGREDIENT_TYPE.MEAT);
        grid[0][0].GetComponent<GridScript>().SetGridIngredients(ing); ;
        grid[0][1].GetComponent<GridScript>().SetGridIngredients(ing);;
        grid[1][1].GetComponent<GridScript>().SetGridIngredients(ing);;
        grid[1][0].GetComponent<GridScript>().SetGridIngredients(ing);;
        break;
    }
  }
}
