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

  GameObject GetGrid(int x, int y)
  {
    return grid[x][y];
  }

  public bool CheckIfLegalMove(GridScript grid, Vector2[] layout)
  {
    for (int i = 0; i < layout.Length; ++i)
    {
      // checking bounds
      Vector2 gridCoordinates = new Vector2(grid.coordinates[0], grid.coordinates[1]);
      Vector2 destGrid = gridCoordinates + layout[i];

      // Outside of grid
      if (destGrid.x < 0.0f || destGrid.x >= gridSizeX ||
        destGrid.y < 0.0f || destGrid.y >= gridSizeY)
      {
        return false;
      }
    }

    return true;
  }

  public void ConstructLevel()
  {
    ConstructGrid(gridSizeX, gridSizeY);
  }

  public void ConstructGrid(int[] gridSizes)
  {
    ConstructGrid(gridSizes[0], gridSizes[1]);
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
}
