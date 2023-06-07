using System;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SpriteRenderer selectedGO;

    [SerializeField]
    private Vector2Int debugPosition;

    private bool selected = false;
    private bool enabledForSelection = true;

    private Action<Tile> OnTileSelected;
    public Vector2Int Pos { get; private set; }
    public int IconId  { get; private set; }
    public bool MarkedForDelete { get; set; }
    public bool CheckedMark { get; set; }
    public Vector2Int prev = Vector2Int.zero;

    public void Init(int iconId, Vector2Int pos, Action<Tile> onTileSelected)
    {
        if( iconId>= sprites.Length )
            return;

        Pos = pos;
        debugPosition = Pos;
        IconId = iconId; 
        spriteRenderer.sprite = sprites[iconId];
        OnTileSelected = onTileSelected;
    }

    public void MoveByY(int y, Action moveDone)
    {
        Debug.Log("MoveByY");
        var target = new Vector3(Pos.x, Pos.y + y, 0);
        Pos = new Vector2Int(Pos.x, Pos.y + y);
        debugPosition = Pos;
        transform.DOLocalMove( target, 1)
            .OnComplete( () => { moveDone.Invoke(); });
    }

    public void MoveTo(Vector2Int pos, Action moveDone)
    {
        Debug.Log("MoveTo");
        Pos = pos;
        debugPosition = Pos;
        transform.DOLocalMove( new Vector3(pos.x, pos.y, 0), 1)
            .OnComplete( () => { moveDone.Invoke(); });
    }

    public bool EnableForSelection 
    {
        set
        {
            enabledForSelection = value;
        }
    }

    public void Selectd(bool selected)
    {
        selectedGO.gameObject.SetActive(selected);
    }
    
    private void OnMouseDown()
    {
        if(!enabledForSelection)
            return;

        selected = !selected;
        selectedGO.gameObject.SetActive(selected);
        OnTileSelected?.Invoke(this);
    }
}
