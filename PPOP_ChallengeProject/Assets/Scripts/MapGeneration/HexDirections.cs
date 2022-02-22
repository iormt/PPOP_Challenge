using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
    public static class HexDirections
    {
        public enum Directions
        {
            L,
            TL,
            TR,
            R,
            BR,
            BL
        }
        public static Dictionary<Directions, Vector3Int> EvenZDirectionOffsets = new Dictionary<Directions, Vector3Int>()
        {
            { Directions.TL, new Vector3Int(-1, 0, 1) },
            { Directions.TR, new Vector3Int(0, 0, 1) },
            { Directions.R, new Vector3Int(1, 0, 0) },
            { Directions.BR, new Vector3Int(0, 0, -1) },
            { Directions.BL, new Vector3Int(-1, 0, -1) },
            { Directions.L, new Vector3Int(-1, 0, 0) }
        };

        public static Dictionary<Directions, Vector3Int> OddZDirectionOffsets = new Dictionary<Directions, Vector3Int>()
        {
            { Directions.TL, new Vector3Int(0, 0, 1) },
            { Directions.TR, new Vector3Int(1, 0, 1) },
            { Directions.R, new Vector3Int(1, 0, 0) },
            { Directions.BR, new Vector3Int(1, 0, -1) },
            { Directions.BL, new Vector3Int(0, 0, -1) },
            { Directions.L, new Vector3Int(-1, 0, 0) }
        };


        public static Array DirectionArray = Enum.GetValues(typeof(HexDirections.Directions));
    }
}
