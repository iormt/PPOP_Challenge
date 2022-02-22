using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

namespace Assets.Scripts.MapGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        #region constants
        const float TILE_X_OFFSET = 1f;
        const float TILE_Z_OFFSET = 0.75f;
        //Water generation
        const int START_OFFSET = 2;
        const int END_OFFSET = 2;
        #endregion

        #region rules
        [Header("General rules")]
        [SerializeField] private int MapWidth = 8;
        [SerializeField] private int MapHeight = 8;

        [SerializeField] private GameObject BaseTilePrefab;
        [SerializeField] private GameObject InaccessibleTilePrefab;
        [SerializeField] private GameObject[] ExtraTilePrefabs;

        [Header("Inaccesible tiles rules")]
        [Tooltip("Deviation probability for inaccesible tile path")]
        [SerializeField] [Range(0, 100)] int PathDeviationProbability = 35;
        [Tooltip("Probability of tiles being placed")]
        [SerializeField] [Range(0, 100)] int TilePlacementProbability = 70;

        [Header("Extra tiles rules")]
        [SerializeField] [Range(0, 8)] int NumberOfSeeds;
        [SerializeField] [Range(0, 100)] int SpreadProbability;
        #endregion

        private Dictionary<Vector3, Tile> _tileDictionary = new Dictionary<Vector3, Tile>();
        private List<Tile> _currentPath = new List<Tile>();

        private Tile _startTile = null;
        private Tile _endTile = null;

        private void OnDestroy()
        {
            Tile.OnTileClicked -= OnTileClicked;
        }
        private void Start()
        {
            Init();
            FillExtraTileTypes();
            FillInaccesibleTiles();
            FillMapWithBaseTile();
            CreatePathfindingGrid();
        }
        private void Update()
        {
            if (Input.GetButtonDown("Cancel") && _startTile && _endTile)
            {
                DehighlightPath(_startTile, _endTile, _currentPath);
                _startTile.DehighlightTile();
                _startTile = null;
                _endTile = null;
            }
        }        
        //TODO: Refactor OnTileClicked
        private void OnTileClicked(Tile tile)
        {
            if (!tile) return;

            if (!_startTile)
            {
                _startTile = tile;
                _startTile.HighlightTile(true);
            }
            else if (_startTile && !_endTile)
            {
                _endTile = tile;
                _currentPath = GetTilePath(); 
                HighlightPath(_startTile, _endTile, _currentPath);
            }
            else if (_startTile && _endTile)
            {
                DehighlightPath(_startTile, _endTile, _currentPath);
                _endTile = tile;
                _currentPath = GetTilePath();
                HighlightPath(_startTile, _endTile, _currentPath);
            }
        }

        private List<Tile> GetTilePath()
        {
            List<Tile> tilePath = new List<Tile>();

            foreach(Tile node in AStar.GetPath(_startTile, _endTile))
            {
                tilePath.Add(node);
            }

            return tilePath;
        }

        private void HighlightPath(Tile startTile, Tile endTile, List<Tile> path)
        {
            for(int i = 0; i < path.Count; i++)
            {
                if (path[i] == endTile)
                {
                    path[i].HighlightTile(true);
                    return;
                }
                if (i + 1 < path.Count 
                    && path[i + 1].GetCost() >= InaccessibleTilePrefab.GetComponent<Tile>().GetCost())
                {

                    _endTile = path[i];
                    path[i].HighlightTile(true);
                    return;
                }
                if (i + 1 < path.Count && path[i + 1].GetCost() < InaccessibleTilePrefab.GetComponent<Tile>().GetCost())
                {
                    if (path[i].GetInstanceID() != startTile.GetInstanceID()
                        && path[i].GetInstanceID() != endTile.GetInstanceID()) path[i].HighlightTile(false);
                }
            }
        }
        private void DehighlightPath(Tile startTile, Tile endTile, List<Tile> path)
        {
            foreach (Tile node in path)
            {
                if (node.GetInstanceID() != startTile.GetInstanceID()) node.DehighlightTile();
            }
        }

        private void Init()
        {
            Tile.OnTileClicked += OnTileClicked;
        }
        private void FillExtraTileTypes()
        {
            List<Vector3> seeds = GenerateSeeds();
            Dictionary<Vector3, GameObject> seedsDict = CreateTilesFromSeeds(seeds);
            SpreadTileSeeds(seedsDict);
        }

        private void SpreadTileSeeds(Dictionary<Vector3, GameObject> seedsDesc)
        {
            foreach(Vector3 seed in seedsDesc.Keys)
            {
                Dictionary<HexDirections.Directions, Vector3Int> neighbourOffsets;
                if (seed.z % 2 == 0)
                {
                    neighbourOffsets = HexDirections.EvenZDirectionOffsets;
                }
                else
                {
                    neighbourOffsets = HexDirections.OddZDirectionOffsets;
                }

                foreach (HexDirections.Directions key in neighbourOffsets.Keys)
                {
                    Vector3 newTileCoordinates = seed + neighbourOffsets[key];
                    if (newTileCoordinates.x > 0 && newTileCoordinates.x < MapWidth 
                        && newTileCoordinates.z > 0 && newTileCoordinates.z < MapHeight)
                    {
                        int probability = UnityEngine.Random.Range(1, 101);
                        if (probability <= SpreadProbability && SpreadProbability > 0)
                        {
                            newTileCoordinates = seed + neighbourOffsets[key];
                            if (!_tileDictionary.ContainsKey(newTileCoordinates))
                            {
                                CreateTile(newTileCoordinates, seedsDesc[seed]);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<Vector3, GameObject> CreateTilesFromSeeds(List<Vector3> seeds)
        {
            Dictionary<Vector3, GameObject> seedTilePrefabDictionary = new Dictionary<Vector3, GameObject>();
            foreach (Vector3 seed in seeds)
            {
                GameObject seedTilePrefab = ExtraTilePrefabs[UnityEngine.Random.Range(0, ExtraTilePrefabs.Length)];
                CreateTile(seed, seedTilePrefab);
                seedTilePrefabDictionary.Add(seed, seedTilePrefab);
            }
            return seedTilePrefabDictionary;
        }

        private List<Vector3> GenerateSeeds()
        {
            List<Vector3> seeds = new List<Vector3>();

            for(int i = 0; i < NumberOfSeeds; i++)
            {
                Vector3 randomPositon = new Vector3(UnityEngine.Random.Range(0, MapWidth), 
                                                    0, UnityEngine.Random.Range(0, MapHeight));
                if (_tileDictionary.ContainsKey(randomPositon) || seeds.Contains(randomPositon))
                {
                    i--;
                }
                else
                {
                    seeds.Add(randomPositon);
                }
            }
            return seeds;

        }
        private void FillInaccesibleTiles()
        {
            //if topToBottom -> L (0, Y) Y > (MapHeight-1)/2 or T (X, 7) and X <= (MapWidth-1)/2
            //                  R (7, Y) Y <= (MapHeight-1)/2 or B (X, 0) and X > (MapWidth-1)/2
            //
            //else -----------> L (0, Y) Y <= (MapHeight-1)/2 or B (X, 0) and X <= (MapWidth-1)/2
            //                  R (7, Y) Y > (MapHeight-1)/2 or T (X, 7) and X > (MapWidth-1)/2
            bool topToBottom = UnityEngine.Random.Range(0, 2) == 1;
            bool startFromLeftSide = UnityEngine.Random.Range(0, 2) == 1; //if false, start from Bottom or Top (depending if topToBottom is true or not)
            bool endInRightSide = UnityEngine.Random.Range(0, 2) == 1; //same as StarFromtLeftSide

            Vector3 startingPosition;
            Vector3 endingPosition;

            GetRandomBoundaryPositions(topToBottom, startFromLeftSide, endInRightSide, out startingPosition, out endingPosition);
            CreateDirectTilePath(startingPosition, endingPosition, InaccessibleTilePrefab);

        }
        private void GetRandomBoundaryPositions(bool topToBottom, bool startFromLeftSide, bool endInRightSide, out Vector3 startingPosition, out Vector3 endingPosition)
        {
            if (topToBottom)
            {
                if (startFromLeftSide)
                {
                    startingPosition = new Vector3(0, 0, UnityEngine.Random.Range((MapHeight - START_OFFSET) / 2, MapHeight - END_OFFSET));
                }
                else
                {
                    startingPosition = new Vector3(UnityEngine.Random.Range(START_OFFSET, (MapWidth + END_OFFSET) / 2), 0, MapHeight - 1);
                }
                if (endInRightSide)
                {
                    endingPosition = new Vector3(MapWidth - 1, 0, UnityEngine.Random.Range(START_OFFSET, (MapHeight + END_OFFSET) / 2));
                }
                else
                {
                    endingPosition = new Vector3(UnityEngine.Random.Range((MapWidth - START_OFFSET) / 2, MapWidth - END_OFFSET), 0, 0);
                }
            }
            else
            {
                if (startFromLeftSide)
                {
                    startingPosition = new Vector3(0, 0, UnityEngine.Random.Range(START_OFFSET, (MapHeight + END_OFFSET) / 2));
                }
                else
                {
                    startingPosition = new Vector3(UnityEngine.Random.Range(START_OFFSET, (MapWidth + END_OFFSET) / 2), 0, 0);
                }
                if (endInRightSide)
                {
                    endingPosition = new Vector3(MapWidth - 1, 0, UnityEngine.Random.Range((MapHeight - START_OFFSET) / 2, MapHeight - END_OFFSET));
                }
                else
                {
                    endingPosition = new Vector3(UnityEngine.Random.Range((MapWidth - START_OFFSET) / 2, MapWidth - END_OFFSET), 0, MapHeight - 1);
                }
            }
            
        }
        private void FillMapWithBaseTile()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int z = 0; z < MapHeight; z++)
                {
                    if (!_tileDictionary.ContainsKey(new Vector3(x, 0, z)))
                    {
                        CreateTile(new Vector3(x, 0, z), BaseTilePrefab);
                    }
                }
            }
        }
        private void CreatePathfindingGrid()
        {
            foreach (Vector3 coordinate in _tileDictionary.Keys)
            {
                SetTileNeighbours(_tileDictionary[coordinate]);
            }
        }
        private GameObject CreateTile(Vector3 coordinates, GameObject tilePrefab)
        {
            GameObject tile = Instantiate(tilePrefab);
            PlaceTile(tile, coordinates);
            InitTile(tile, coordinates);
            return tile;
        }
        private void PlaceTile(GameObject tile, Vector3 coordinates)
        {
            if (coordinates.z % 2 == 0)
            {
                tile.transform.position = new Vector3(coordinates.x * TILE_X_OFFSET, 0f, coordinates.z * TILE_Z_OFFSET);
            }
            else
            {
                tile.transform.position = new Vector3(coordinates.x * TILE_X_OFFSET + TILE_X_OFFSET / 2, 0f, coordinates.z * TILE_Z_OFFSET);
            }
        }
        private void InitTile(GameObject tile, Vector3 coordinates)
        {
            tile.GetComponent<Tile>().Init(coordinates);
            tile.transform.parent = transform;
            _tileDictionary.Add(coordinates, tile.GetComponent<Tile>());
        }
        private Tile FindTileByCoordinates(Vector3 tileCoordinates)
        {
            Tile tile;
            _tileDictionary.TryGetValue(tileCoordinates, out tile);
            return tile ? tile.GetComponent<Tile>() : null;
        }
        private void SetTileNeighbours(Tile tile)
        {
            Vector3 tileCoordinates = tile.GetCoordinates();
            if (tile.GetCoordinates().z % 2 == 0)
            {
                ProcessTileNeighbours(tile, tileCoordinates, HexDirections.EvenZDirectionOffsets);
            }
            else
            {
                ProcessTileNeighbours(tile, tileCoordinates, HexDirections.OddZDirectionOffsets);
            }
        }
        private void ProcessTileNeighbours(Tile tile, Vector3 tileCoordinates, Dictionary<HexDirections.Directions, Vector3Int> neighbourOffsetArray)
        {
            foreach (HexDirections.Directions key in neighbourOffsetArray.Keys)
            {
                Tile neighbour = FindTileByCoordinates(tileCoordinates + neighbourOffsetArray[key]);
                if (neighbour) tile.GetNeighbours().Add(neighbour);
            }
        }
        private void CreateDirectTilePath(Vector3 start, Vector3 end, GameObject tilePrefab)
        {
            CreateTile(start, tilePrefab);
            Vector3 nextStepCoordinates = start;

            while ((!nextStepCoordinates.Equals(end))) //while position is different from target
            {
                int deviationProbability = UnityEngine.Random.Range(1, 101);
                int tilePlacementProbability = UnityEngine.Random.Range(1, 101);
                int randDirectionIndex = UnityEngine.Random.Range(0, 6);

                bool placeTile = tilePlacementProbability <= TilePlacementProbability && TilePlacementProbability > 0;

                HexDirections.Directions randomDirection = 
                    (HexDirections.Directions)HexDirections.DirectionArray.GetValue(randDirectionIndex);
                if (deviationProbability <= PathDeviationProbability || PathDeviationProbability == 0)
                {
                    nextStepCoordinates = GetNextStepCoordinates(end, nextStepCoordinates);
                }
                else
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[randomDirection] :
                        HexDirections.OddZDirectionOffsets[randomDirection];
                }
                if (!(nextStepCoordinates.x < 0 || nextStepCoordinates.x >= MapWidth
                    || nextStepCoordinates.z < 0 || nextStepCoordinates.z >= MapHeight))
                {
                    if (!_tileDictionary.ContainsKey(nextStepCoordinates) && placeTile)
                    {
                        CreateTile(nextStepCoordinates, tilePrefab);
                    }
                }
            }
        }
        private static Vector3 GetNextStepCoordinates(Vector3 end, Vector3 nextStepCoordinates)
        {
            if (nextStepCoordinates.z != end.z) //if both axes are different, move diagonally towards the target
            {
                if (nextStepCoordinates.x < end.x && nextStepCoordinates.z < end.z)
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[HexDirections.Directions.TR] :
                        HexDirections.OddZDirectionOffsets[HexDirections.Directions.TR];
                }
                else if (nextStepCoordinates.x < end.x && nextStepCoordinates.z > end.z)
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[HexDirections.Directions.BR] :
                        HexDirections.OddZDirectionOffsets[HexDirections.Directions.BR];
                }
                else if (nextStepCoordinates.x > end.x && nextStepCoordinates.z < end.z)
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[HexDirections.Directions.TL] :
                        HexDirections.OddZDirectionOffsets[HexDirections.Directions.TL];
                }
                else if (nextStepCoordinates.x > end.x && nextStepCoordinates.z > end.z)
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[HexDirections.Directions.BL] :
                        HexDirections.OddZDirectionOffsets[HexDirections.Directions.BL];
                }
                else if (nextStepCoordinates.x == end.x && nextStepCoordinates.z < end.z)
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[HexDirections.Directions.TR] :
                        HexDirections.OddZDirectionOffsets[HexDirections.Directions.TL];
                }
                else if (nextStepCoordinates.x == end.x && nextStepCoordinates.z > end.z)
                {
                    nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                        HexDirections.EvenZDirectionOffsets[HexDirections.Directions.BR] :
                        HexDirections.OddZDirectionOffsets[HexDirections.Directions.BL];
                }
            }
            else if (nextStepCoordinates.z == end.z && nextStepCoordinates.x != end.x)//move in a straight line 
            {
                int xMovementDir = nextStepCoordinates.x < end.x ? 1 : -1;
                nextStepCoordinates = new Vector3(nextStepCoordinates.x + xMovementDir,
                                                nextStepCoordinates.y, nextStepCoordinates.z);
            }

            return nextStepCoordinates;
        }
    }
}