using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

namespace Kyoto
{
    public class TileController : Singleton<TileController>
    {
        // public List<Tile[,,]> tiles = new List<Tile[,,]>();
        public Tile[,] tiles;

        protected override void OnRegistration ()
        {
            // Debug.Log ("Singleton ready!");
            tiles = new Tile[5,5];
        }

        public void AddTile(Tile inTile, Vector2Int inPosition)
        {
            // Debug.Log("AddTile: " + inTile.gameObject.name + " at " + inPosition, inTile);
            // Debug.Log(tiles, this);
            tiles[inPosition.x, inPosition.y] = inTile;
        }

        public Tile GetTile(Vector2Int inPosition)
        {
            return tiles[inPosition.x, inPosition.y];
        }

        public void SetTileOccupancy(Vector2Int inPosition, Placeable inPlaceable)
        {
            // GetTile(inPosition).GetComponent<TileOccupancy>().occupier = inPlaceable;

            // if (inPlaceable)
            // {
                // foreach (Vector2Int v in inPlaceable)
                // {
                //     if (inPlaceable)
                //     {
                //         Debug.Log("SetTileOccupancy: " + v, inPlaceable);
                //     } else {
                //         Debug.Log("SetTileOccupancy: " + v);
                //     }
                //
                //     GetTile(v).GetComponent<TileOccupancy>().occupier = inPlaceable;
                // }
            // }

            GetTile(inPosition).GetComponent<TileOccupancy>().SetOccupier(inPlaceable);
        }
    }
}
