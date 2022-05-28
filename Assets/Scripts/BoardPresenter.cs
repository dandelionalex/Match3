using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BoardPresenter : MonoBehaviour
{
    [SerializeField]
    private Tile tilePrefab;

    private Tile[,] tiles = null;

    private Tile _selectedTile;

    public void GenerateBoard(int widht, int height)
    {
        foreach(Transform tr in transform)
        {
            Destroy(tr.gameObject);
        }

        tiles = new Tile[widht, height];

        for(var x = 0; x<widht; x++)
        {
            for(var y = 0; y<height; y++)
            {
                var tile = Instantiate(tilePrefab);
                tile.transform.SetParent( this.transform, false );
                tile.transform.position = new Vector3(x, y, 0 );
                tile.Init(UnityEngine.Random.Range(0,4), new Vector2Int( x, y ), OnTileSelected);
                tiles[ x, y ] = tile;
            }    
        }  

        transform.position = new Vector3(widht / -2 + 0.5f, height / -2 + 0.5f, 0); 
    }

    private int totalMoves = 0;

    private void OnTileSelected(Tile selectedTile)
    {
        if(selectedTile == _selectedTile)
        {
            _selectedTile = null;
            return;  
        }

        if(_selectedTile != null)
        {
            Debug.Log("neib, swap");
            totalMoves++;
            selectedTile.MoveTo(_selectedTile.Pos, () => {
                selectedTile.UpdatePositions(_selectedTile.Pos);
                Debug.Log("move done");
                MoveDone();
            });
            totalMoves++;
            _selectedTile.MoveTo(selectedTile.Pos, () => {
                _selectedTile.UpdatePositions(selectedTile.Pos);
                Debug.Log("move done 2");
                MoveDone();
            });
            return;
        }

        _selectedTile = selectedTile;
        foreach(var tile in tiles)
        {
            tile.EnableForSelection = IsNeighbor( selectedTile.Pos, tile.Pos );
        }
    }

    private void MoveDone()
    {
        totalMoves--;
        if(totalMoves<=0)
        {
            CheckForMathes();
        }
    }

    private bool IsNeighbor(Vector2Int posOne, Vector2Int posTwo)
    {
        foreach (var n in NeighborsPos)
        {
            if( posOne - n == posTwo)
                return true;
        }

        return false;
    }

    private void CheckForMathes()
    {
        Debug.Log("CheckForMathes");
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);

        // check for column
        for( int x = 0; x< width; x++ )
        {
            int col = 1;
            int last = -1;
            for( int y = 0; y< height; y++ )
            {
                if(last == -1)
                {
                    last = tiles[x,y].IconId;
                    continue;
                }

                if(tiles[x,y].IconId == last)
                {
                    col++;
                }

                if( tiles[x,y].IconId != last || y == height-1 )
                {
                    if(col>=3)
                    {
                        var t = y-col;

                        for(int i = --y; i >= t; i--)
                        {
                             tiles[x,i].MarkedForDelete = true;   
                             Debug.Log("mard for delete");
                             tiles[x,i].SelectD();
                        }
                    }
                    col = 1;
                }
                
                last = tiles[x,y].IconId;
            }
        }
    }

    private List<Vector2Int> NeighborsPos = new List<Vector2Int>{ 
        Vector2Int.left, 
        Vector2Int.right, 
        Vector2Int.up, 
        Vector2Int.down };
}
