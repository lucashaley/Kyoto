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
            try
            {
                return tiles[inPosition.x, inPosition.y];
            }
            catch
            {
                return null;
            }
        }

        public void SetTileOccupancy(Vector2Int inPosition, Placeable inPlaceable)
        {
            // check if tile is null
            GetTile(inPosition)?.GetComponent<TileOccupancy>().SetOccupier(inPlaceable);
        }

        // this is the prototype relative version -- still needed?
        public bool CheckTileOccupancy(Vector2Int origin, Vector2Int footprint)
        {
            // Debug.Log("CheckTileOccupancy: " + origin + " and " + footprint);
            for (int x = 0; x < footprint.x; x++)
            {
                for (int y = 0; y < footprint.y; y++)
                {
                    if (GetTile(new Vector2Int(origin.x + x, origin.y + y)).GetComponent<TileOccupancy>().IsOccupied())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // this is the absolute version
        public bool CheckTileOccupancyByPosition(Vector2Int start, Vector2Int end, Placeable placeable = null)
        {
            Vector2Int iterator = end - start;
            Vector2Int root = start;
            if (iterator.x < 0 || iterator.y < 0)
            {
                iterator = start - end;
                root = end;
            }
            Vector2Int tileIterator = end - start;
            // Debug.Log("Iterator: " + iterator);

            for (int x = 0; x <= Mathf.Abs(iterator.x); x++)
            {
                for (int y = 0; y <= Mathf.Abs(iterator.y); y++)
                {
                    Tile currentTile = GetTile(new Vector2Int(root.x + x, root.y + y));
                    if (currentTile != null)
                    {
                        // Debug.Log("Checking tile: " + currentTile.gameObject.name, currentTile.gameObject);
                        TileOccupancy currentTileOccupancy = currentTile.GetComponent<TileOccupancy>();
                        if (currentTileOccupancy.occupier != placeable && currentTileOccupancy.IsOccupied())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SetTileOccupancyByPosition(Vector2Int start, Vector2Int end, Placeable placeable = null)
        {
            Vector2Int iterator = end - start;
            Vector2Int root = start;
            if (iterator.x < 0 || iterator.y < 0)
            {
                iterator = start - end;
                root = end;
            }
            Vector2Int tileIterator = end - start;
            // Debug.Log("Iterator: " + iterator);

            for (int x = 0; x <= Mathf.Abs(iterator.x); x++)
            {
                for (int y = 0; y <= Mathf.Abs(iterator.y); y++)
                {
                    Tile currentTile = GetTile(new Vector2Int(root.x + x, root.y + y));
                    // Debug.Log("Setting tile: " + currentTile.gameObject.name, currentTile.gameObject);
                    currentTile?.GetComponent<TileOccupancy>().SetOccupier(placeable);
                }
            }
        }
    }
}
