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
        IconId = iconId; 
        spriteRenderer.sprite = sprites[iconId];
        OnTileSelected = onTileSelected;
    }

    public void MoveTo(Vector2Int pos, Action moveDone)
    {
        transform.DOLocalMove( new Vector3(pos.x, pos.y, 0), 1)
            .OnComplete( () => { moveDone.Invoke(); });
    }

    public void UpdatePositions(Vector2Int pos)
    {
        this.Pos = pos;
    }

    public bool EnableForSelection 
    {
        set
        {
            enabledForSelection = value;
        }
    }

    public void SelectD()
    {
        selectedGO.gameObject.SetActive(true);
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
