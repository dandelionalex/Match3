using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoardPresenter : MonoBehaviour
{
    public int LeftToWin {get; private set;}
    public Action<int> matchCountUpdated;

    [SerializeField]
    private Tile tilePrefab;
    private int _totalAnimationMoves = 0;
    private List<Tile> _tiles = new List<Tile>();
    private Tile _selectedTile;
    private Vector2Int _boardSize;
    private List<List<Tile>> _itemsForRenew = new List<List<Tile>>();

    private GameSession _gameSession;
    private int _variaty;
    private bool _cheatModeEnabled = false;

    private void IncreaseMathes()
    {
        LeftToWin--;        
        matchCountUpdated?.Invoke(LeftToWin);
    }

    private void Awake()
    {
        _gameSession = FindObjectOfType<GameSession>();
    }

    private void CheckForFinish()
    {
        if(LeftToWin <= 0)
        {
            LeftToWin = 0;
            _gameSession.FinishGame();
            return;
        }
    }

    public void GenerateBoard(int widht, int height, int leftToWin, int variaty)
    {
        LeftToWin = leftToWin;
        _boardSize = new Vector2Int(widht, height);
        _variaty = variaty;
        
        for(var x = 0; x<widht; x++)
        {
            for(var y = 0; y<height; y++)
            {        
                var pos = new Vector2Int( x, y );
                var icon = GetIcon(_tiles, pos);
                AddTileItem(pos, icon);
            }    
        }  

        transform.position = new Vector3(widht / -2 + 2.5f, height / -2 + 0.5f, 0); 
    }

    public void CheatModeEnabled()
    {
        _cheatModeEnabled = true;
    }

    public void Reset()
    {
        foreach(Transform tr in transform)
        {
            Destroy(tr.gameObject);
        }

        _tiles.Clear();
    }

    private Tile AddTileItem( Vector2Int pos, int icon)
    {
        var tile = Instantiate(tilePrefab);
        tile.transform.SetParent( this.transform, false );
        tile.transform.localPosition = new Vector3( pos.x, pos.y, 0 );
        tile.Init(icon, pos, OnTileSelected);
        _tiles.Add(tile);
        return tile;
    }

    private int GetIcon(List<Tile> tiles, Vector2Int pos)
    {
        var posibilities = GetVariatyList(_variaty);
        var left = tiles.Where( t => t.Pos == pos + Vector2Int.left || t.Pos == pos + Vector2Int.left * 2 ).ToList();
        var bottom = tiles.Where( t => t.Pos == pos + Vector2Int.down || t.Pos == pos + Vector2Int.down * 2 ).ToList();
        
        if(left.Count == 2 && left[0].IconId == left[1].IconId )
            posibilities.Remove(left[0].IconId);
        
        if(bottom.Count == 2 && bottom[0].IconId == bottom[1].IconId )
            posibilities.Remove(bottom[0].IconId);
        
        if(posibilities.Count == 0)
           return UnityEngine.Random.Range(0, _variaty);

        return posibilities[ UnityEngine.Random.Range(0, posibilities.Count) ];
    }

    private int GetTileReplacementIcon(List<Tile> tiles, Vector2Int pos)
    {
        var posibilities = GetVariatyList(_variaty);
        var left = tiles.Where( t => t.Pos == pos + Vector2Int.left || t.Pos == pos + Vector2Int.left * 2 ).ToList();
        var bottom = tiles.Where( t => t.Pos == pos + Vector2Int.down || t.Pos == pos + Vector2Int.down * 2 ).ToList();
        var top = tiles.Where( t => t.Pos == pos + Vector2Int.right || t.Pos == pos + Vector2Int.right * 2 ).ToList();
        var right = tiles.Where( t => t.Pos == pos + Vector2Int.up || t.Pos == pos + Vector2Int.up * 2 ).ToList();
       
        checkPossobility(left, posibilities);
        checkPossobility(bottom, posibilities);
        checkPossobility(top, posibilities);
        checkPossobility(right, posibilities);
        
        Debug.Log($"GetTileReplacementIcon: {posibilities.Count}, _variaty: {_variaty}");

        if(posibilities.Count == 0)
           return UnityEngine.Random.Range(0, _variaty);

        return posibilities[ UnityEngine.Random.Range(0, posibilities.Count) ];
    }

    private void checkPossobility(List<Tile> l, List<int> possoblities)
    {
        if(l.Count == 2 && l[0].IconId == l[1].IconId)
            possoblities.Remove(l[0].IconId);
    }

    private void OnTileSelected(Tile selectedTile)
    {
        if(selectedTile == _selectedTile)
        {
            _selectedTile = null;
            EnambleAllTiles();
            return;  
        }

        if(_selectedTile != null)
        {
            var tileOnePos = _selectedTile.Pos;
            var tileTwoPos = selectedTile.Pos;
            
            _totalAnimationMoves++;
            selectedTile.Selectd(false);
            selectedTile.MoveTo(tileOnePos, () => {
                PlayerMoveDone();
            });

            _totalAnimationMoves++;
             _selectedTile.Selectd(false);
            _selectedTile.MoveTo(tileTwoPos, () => {
                PlayerMoveDone();
            });
            _selectedTile = null;
            return;
        }

        _selectedTile = selectedTile;
        foreach(var tile in _tiles)
        {
            tile.EnableForSelection = IsNeighbor( selectedTile.Pos, tile.Pos );
        }
    }

    private void PlayerMoveDone()
    {
        _totalAnimationMoves--;
        if(_totalAnimationMoves <= 0)
        {
            CheckForMathes();
        }
    }

    private void ShiftDone()
    {
        _totalAnimationMoves--;
        if(_totalAnimationMoves != 0)
            return;

        GenerateReplacement();
    }

    private void GenerateReplacement()
    {
        int x = 0;
        foreach(var item in _itemsForRenew)
        {
            for(var i =0; i< item.Count; i++ )
            {
                var pos = new Vector2Int( x, _boardSize.y-1-i );
                var icon = _cheatModeEnabled ? GetIcon(_tiles, pos) : GetTileReplacementIcon(_tiles, pos);
                AddTileItem(pos, icon);
            }
            x++;
        }    
        _itemsForRenew.Clear();

        if(LeftToWin <= 0)
        {
            CheckForFinish();
            return;
        }

        CheckForMathes();
    }

    private void CheckForMathes()
    {
        List<List<Tile>> hLines = new List<List<Tile>>();
        for(int x = 0; x< _boardSize.x; x++)
        {
            var l = _tiles.Where<Tile>(t => t.Pos.x == x).ToList();
            l.Sort((x,y) => x.Pos.y.CompareTo(y.Pos.y));
            hLines.Add(l);
        }
        CheckForMathes(hLines);

        List<List<Tile>> wLines = new List<List<Tile>>();
        for(int y = 0; y< _boardSize.y; y++)
        {
            var l = _tiles.Where<Tile>(t => t.Pos.y == y).ToList();
            l.Sort((x,y) => x.Pos.x.CompareTo(y.Pos.x));
            wLines.Add(l);
        }
        CheckForMathes(wLines);

        _itemsForRenew.Clear();
        foreach(var tColumn in hLines)
        {
            int shift = 0;
            var column = new List<Tile>();
            _itemsForRenew.Add(column);
            foreach( var tile in tColumn )
            {
                if(tile.MarkedForDelete)
                {
                    shift++;
                    column.Add(tile);
                    continue;
                }
                
                if(shift>0)
                {
                    _totalAnimationMoves++;
                    tile.MoveByY(shift*-1, () => { ShiftDone(); });  
                }                          
            }   
        }

        foreach(var col in _itemsForRenew)
        {
            foreach(var t in col)
            {
                Destroy(t.gameObject);
                _tiles.Remove(t);
            }
        }

        var itemsForRenewCount = _itemsForRenew.Sum(col => col.Count());
        Debug.Log($"itemsForRenewCount: {itemsForRenewCount}, totalAnimationMoves: {_totalAnimationMoves}");

        if(_totalAnimationMoves == 0 && itemsForRenewCount > 0)
        {
            GenerateReplacement();
        }
        else if(itemsForRenewCount == 0)
        {
            EnambleAllTiles();
        }
    }

    private void EnambleAllTiles()
    {
        foreach(var tile in _tiles)
        {
            tile.EnableForSelection = true;
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

    private void CheckForMathes(List<List<Tile>> formatedTiles)
    {
        foreach(var row in formatedTiles)
        {
            int repeate = 1;
            int last = -1;
            for(var i=0; i < row.Count; i++)
            {
                if(last == -1)
                {
                    last = row[i].IconId;
                    continue;
                }

                if(row[i].IconId == last)
                    repeate++;

                if( row[i].IconId != last  )
                {
                    if(repeate >= 3)
                        MarkToDelete(row, i - repeate, i-1);
                    
                    repeate = 1;
                    last = row[i].IconId;
                    continue;
                }

                if(i == row.Count-1 && repeate>=3)
                    MarkToDelete(row, i - repeate + 1, i);
            }        
        }        
    }

    private List<int> GetVariatyList( int variaty)
    {
        var variatyList = new List<int>();
        for(int i = 0; i <= variaty; i++)
            variatyList.Add(i);

        return variatyList;
    }        

    private void MarkToDelete(List<Tile> row, int sPos, int ePos)
    {
        for(int n = sPos; n <= ePos; n++)
        {
            row[n].MarkedForDelete = true;   
            row[n].Selectd(true);
        }

        IncreaseMathes();
    }

    private List<Vector2Int> NeighborsPos = new List<Vector2Int>{ 
        Vector2Int.left, 
        Vector2Int.right, 
        Vector2Int.up, 
        Vector2Int.down };
}
