using block_racing_common.Game.Pieces;
using block_racing_common.Game.Snapshots;
using System.Linq;
using UnityEngine;

public class FlyingBlockView : MonoBehaviour
{
    [SerializeField]
    private BlockView[] cells;


    private const float CellWidth = 100f;
    private const float CellHeight = 55f;

    private const int LaneHeight = 20;

    private RectTransform _rect;


    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }


    public void UpdateBlock(
        FlyingBlockSnapshot snapshot)
    {
        _rect.anchoredPosition =
            new Vector2(
                snapshot.X * CellWidth,
                snapshot.Y * CellHeight
            );

        Debug.Log(
            $"FlyingBlock RectPos : {_rect.anchoredPosition}, Snapshot : {snapshot.X},{snapshot.Y}"
        );

        UpdateShape(snapshot);
    }


    private void UpdateShape(
        FlyingBlockSnapshot snapshot)
    {
        CellPosition[] shape =
            PieceShapeTable.GetShape(
                snapshot.Type,
                snapshot.Rotation);

        int minX = shape.Min(cell => cell.X);
        int minY = shape.Min(cell => cell.Y);

        for (int i = 0; i < cells.Length; i++)
        {
            if (i >= shape.Length)
            {
                cells[i].SetBlock(0);
                continue;
            }


            cells[i].SetBlock(1);


            RectTransform cellRect =
                cells[i].GetComponent<RectTransform>();

            cellRect.anchorMin = Vector2.zero;
            cellRect.anchorMax = Vector2.zero;
            cellRect.pivot = Vector2.zero;

            // Piece ³»ºÎ ÁÂÇ¥
            cellRect.anchoredPosition =
                new Vector2(
                    (shape[i].X - minX) * CellWidth,
                    (shape[i].Y - minY) * CellHeight
                );
        }
    }
}