using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Mirror;
using Multiplayer;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;
        public Vector2Int gridSize;
        public static readonly Vector2 CellSize = new Vector2(0.18f, 0.18f); //Bounds of sprite

        [SerializeField]
        public const int GridBoundX = 256;
        [SerializeField]
        public const int GridBoundY = 256;

        [SerializeField] private LandScapeCellSprites[] landScapeCells;
        Dictionary<LandScapeCellSprites, Color> _avgLandScapeCellColors = new Dictionary<LandScapeCellSprites, Color>();
         public Texture2D mapTexture;
         public string mapPath;

        public List<GridCell> gridCells = new List<GridCell>();
        public List<GridCell> lightUpCells = new List<GridCell>();
        public static event Action GridGenerated;

        private Camera _camera;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (UpdateLandScapeCells() == 0)
            {
                Debug.LogError("No LandScapeCells found");
            }
            
            foreach (var landScapeCell in landScapeCells)
            {
                _avgLandScapeCellColors.Add(landScapeCell, Texture2DHelper.AverageColorWithoutOutliers(landScapeCell.sprite.texture.GetRawTextureData()));
            }
            
        }

        


        void Start()
        {
            _camera = Camera.main;
        }
        
        

        public void GenerateGridFromJson()
        {
            
            if (mapPath == "")
            {
                NetworkGameManager.Instance.UpdateState(GameState.ChoosingMap);
                mapPath = OpenFileHelper.GetPathToLoadJsonFile();
            }
            
            //Load from file
            string json = System.IO.File.ReadAllText(mapPath);
        
            GenerateGridFromJson(json);
        }

        public void GenerateGridFromJson(string json)
        {
            //Deserialize JSON to grid
            NetworkGameManager.Instance.UpdateState(GameState.GeneratingMap);

            var serializedGridCells = JsonHelper.FromJson<GridCellSerialization>(json);
        
            if (serializedGridCells.Length == 0)
            {
                Debug.Log("Grid is empty");
                return;
            }
            
            foreach (var serializedGridCell in serializedGridCells)
            {
                if (gridCells.Count() == GridBoundX * GridBoundY)
                {
                    Debug.Log("Grid is full");
                    break;
                }
                serializedGridCell.CreateGridCell();
            }
        
            Instance.GridSuccessfullyGenerated();
        }

        public static string MapToJson(List<GridCell> mapCells)
        {
            if (mapCells.Count == 0)
            {
                Debug.Log("No grid to save");
                return "";
            }

            var serializedGridCells = new GridCellSerialization[mapCells.Count];

            for(int i = 0; i < mapCells.Count; i++)
            {
                serializedGridCells[i] = new GridCellSerialization(mapCells[i]);
            }

            return JsonHelper.ToJson(serializedGridCells, false);
        }

        public void GenerateGrid()
        {
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var cellPosition = new Vector3(x * CellSize.x, y * CellSize.y);
                    Instantiate(landScapeCells[0], cellPosition, Quaternion.identity);
                }
            }

            GridSuccessfullyGenerated();
        }

        public void GenerateFromImage()
        {
            
            var map = Texture2DHelper.MakeTextureAppropriate(mapTexture, GridBoundX, GridBoundY, 18, 18,
                TextureFormat.RGB24);
            var mapWidth = map.width / 18; //18 - Pixel width of sprite
            var mapHeight = map.height / 18; //18 - Pixel height of sprite
            var mapPixels = map.GetRawTextureData();
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    var cellPosition = new Vector3(x * CellSize.x, y * CellSize.y);

                    //Get 18x18 pixel data from map texture
                    var mapPixelData = new byte[18 * 18 * 3];
                    for (var i = 0; i < 18; i++)
                    {
                        for (var j = 0; j < 18; j++)
                        {
                            var index = (y * 18 + i) * mapWidth * 18 * 3 + (x * 18 + j) * 3;
                            mapPixelData[i * 18 * 3 + j * 3] = mapPixels[index];
                            mapPixelData[i * 18 * 3 + j * 3 + 1] = mapPixels[index + 1];
                            mapPixelData[i * 18 * 3 + j * 3 + 2] = mapPixels[index + 2];
                        }
                    }
                    
                    GameObject cell = null;
                    int minDifference = 255 * 3 + 1;
                    var closestLandScape = landScapeCells[0];

                    foreach (var avgColorLandScape in _avgLandScapeCellColors)
                    {
                        var avgColorMap = Texture2DHelper.AverageColorWithoutOutliers(mapPixelData);
                        var difference =
                            Texture2DHelper.AverageDifferenceBetweenColors(avgColorMap, avgColorLandScape.Value);

                        if (difference < minDifference)
                        {
                            closestLandScape = avgColorLandScape.Key;
                            minDifference = difference;
                        }
                    }
                    
                    cell = new GameObject();
                    cell.transform.position = cellPosition;
                    cell.AddComponent<SpriteRenderer>();
                    cell.AddComponent<GridCell>().landScapeCellSprites = closestLandScape;
                    cell.transform.parent = transform;
                }
            }

            GridSuccessfullyGenerated();
        }

        public void GridSuccessfullyGenerated()
        {
            GridGenerated?.Invoke();
        }
        public void DestroyGrid()
        {
            foreach (var cell in gridCells)
            {
                Destroy(cell.gameObject);
            }

            gridCells.Clear();
        }
        public HashSet<GridCell> GetArea(GridCell cell1, GridCell cell2)
        {
            var x1 = cell1.gridPosition.x;
            var x2 = cell2.gridPosition.x;
            var y1 = cell1.gridPosition.y;
            var y2 = cell2.gridPosition.y;
            var maxX = Mathf.Max(x1, x2);
            var minX = Mathf.Min(x1, x2);
            var maxY = Mathf.Max(y1, y2);
            var minY = Mathf.Min(y1, y2);
            var area = new HashSet<GridCell>();
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    area.Add(GetGridCell(new Vector2Int(x, y)));
                }
            }

            return area;
        }
        
        public  void LightUpArea(HashSet<GridCell> area)
        {
            foreach (var gridCell in area)
            {
                lightUpCells.Add(gridCell);
                gridCell.LightUp();
            }
        }
        
        public  void LightDownArea()
        {
            foreach (var gridCell in lightUpCells)
            {
                gridCell.LightDown();
            }
            lightUpCells.Clear();
        }
        
        private int UpdateLandScapeCells()
        {
            landScapeCells =  Resources.LoadAll<LandScapeCellSprites>("LandScape");
            return landScapeCells.Length;
        }

        public GridCell MouseOverCell()
        {
            if (!_camera)
            {
                return null;
            }
            
            if (gridCells.Count == 0)
            {
                return null;
            }
            
            var point = _camera.ScreenToWorldPoint(Input.mousePosition);
            var x = Mathf.RoundToInt(point.x / CellSize.x);
            var y = Mathf.RoundToInt(point.y / CellSize.y);
            if (x < 0 || x > gridCells.Last().gridPosition.x || y < 0 || y > gridCells.Last().gridPosition.y)
            {
                return null;
            }

            return GetGridCell(new Vector2Int(x, y));
        }
        
        public void AddTile(GameObject tile)
        {
            var gridCell = tile.GetComponent<GridCell>();
            gridCells.Add(gridCell);
        }

        public GridCell GetGridCell(Vector2Int gridPosition)
        {
            return gridCells.Find(cell => cell.gridPosition == gridPosition);
        }

        public static Vector2Int PositionToGridPosition(Vector2 position)
        {
            var x = Mathf.RoundToInt(position.x / CellSize.x);
            var y = Mathf.RoundToInt(position.y / CellSize.y);
            return new Vector2Int(x, y);
        }

        public static Vector2Int PositionToGridPosition(Vector3 position)
        {
            var x = Mathf.RoundToInt(position.x / CellSize.x);
            var y = Mathf.RoundToInt(position.y / CellSize.y);
            return new Vector2Int(x, y);
        }

        public Vector2Int GetLeftDownBorder()
        {   
             return gridCells.First().gridPosition;
        }
        
        public Vector2Int GetRightUpBorder()
        {
            return gridCells.Last().gridPosition;
        }
    }
    
    
    
}