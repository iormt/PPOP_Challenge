using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
using System;

namespace Assets.Scripts.MapGeneration
{
    public class Tile : MonoBehaviour, IAStarNode
    {
        //Properties
        [SerializeField] private float Cost;
        [SerializeField] private Material Material;

        private TileHighlighter _tileHighlighter;
        private Vector3 Coordinates;
        private List<Tile> Neighbours = new List<Tile>();

        IEnumerable<IAStarNode> IAStarNode.Neighbours => Neighbours;

        //Events
        public static event Action<Tile> OnTileClicked;

        //Functions
        public void Init(Vector3 coordinates)
        {
            GetComponent<MeshRenderer>().material = Material;
            _tileHighlighter = GetComponent<TileHighlighter>();
            Coordinates = coordinates;
            name = coordinates.x.ToString() + ", " + coordinates.z.ToString();
            _tileHighlighter.Init();
        }
        public float GetCost()
        {
            return Cost;
        }

        public List<Tile> GetNeighbours()
        {
            return Neighbours;
        }

        public bool IsInCoordinates(Vector3 coordinates)
        {
            return coordinates.Equals(Coordinates);
        }

        public Vector3 GetCoordinates()
        {
            return Coordinates;
        }

        public void HighlightTile(bool isEnd)
        {
            _tileHighlighter.HighlightTile(isEnd);
        }

        public void DehighlightTile()
        {
            _tileHighlighter.DehighlightTile();
        }

        private void OnMouseOver()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                OnTileClicked?.Invoke(this);
            }
        }

        float IAStarNode.CostTo(IAStarNode neighbour)
        {
            return ((Tile)neighbour).GetCost();
        }

        float IAStarNode.EstimatedCostTo(IAStarNode target)
        {
            float estimatedCost = 0;
            
            Vector3 nextStepCoordinates = Coordinates;
            Vector3 targetCoord = ((Tile)target).GetCoordinates();

            while ((!nextStepCoordinates.Equals(targetCoord))) //while position is different from target
            {
                if (nextStepCoordinates.z != targetCoord.z) //if both axes are different, move diagonally towards the target
                {
                    if (nextStepCoordinates.x < targetCoord.x && nextStepCoordinates.z < targetCoord.z)
                    {
                        nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                            HexDirections.EvenZDirectionOffsets[HexDirections.Directions.TR] :
                            HexDirections.OddZDirectionOffsets[HexDirections.Directions.TR];
                    }
                    else if (nextStepCoordinates.x < targetCoord.x && nextStepCoordinates.z > targetCoord.z)
                    {
                        nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                            HexDirections.EvenZDirectionOffsets[HexDirections.Directions.BR] :
                            HexDirections.OddZDirectionOffsets[HexDirections.Directions.BR];
                    }
                    else if (nextStepCoordinates.x > targetCoord.x && nextStepCoordinates.z < targetCoord.z)
                    {
                        nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                            HexDirections.EvenZDirectionOffsets[HexDirections.Directions.TL] :
                            HexDirections.OddZDirectionOffsets[HexDirections.Directions.TL];
                    }
                    else if (nextStepCoordinates.x > targetCoord.x && nextStepCoordinates.z > targetCoord.z)
                    {
                        nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                            HexDirections.EvenZDirectionOffsets[HexDirections.Directions.BL] :
                            HexDirections.OddZDirectionOffsets[HexDirections.Directions.BL];
                    }
                    else if (nextStepCoordinates.x == targetCoord.x && nextStepCoordinates.z < targetCoord.z)
                    {
                        nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                            HexDirections.EvenZDirectionOffsets[HexDirections.Directions.TR] :
                            HexDirections.OddZDirectionOffsets[HexDirections.Directions.TL];
                    }
                    else if (nextStepCoordinates.x == targetCoord.x && nextStepCoordinates.z > targetCoord.z)
                    {
                        nextStepCoordinates += nextStepCoordinates.z % 2 == 0 ?
                            HexDirections.EvenZDirectionOffsets[HexDirections.Directions.BR] :
                            HexDirections.OddZDirectionOffsets[HexDirections.Directions.BL];
                    }
                }
                else if (nextStepCoordinates.z == targetCoord.z && nextStepCoordinates.x != targetCoord.x)//move in a straight line 
                {
                    int xMovementDir = nextStepCoordinates.x < targetCoord.x ? 1 : -1;
                    nextStepCoordinates = new Vector3(nextStepCoordinates.x + xMovementDir,
                                                    nextStepCoordinates.y, nextStepCoordinates.z);
                }
                estimatedCost += 1;
            }
            return estimatedCost;
        }
    }
}