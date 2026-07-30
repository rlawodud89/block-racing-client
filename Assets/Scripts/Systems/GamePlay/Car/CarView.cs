using UnityEngine;
using block_racing_common.Game.Snapshots;

public class CarView : MonoBehaviour
{
    private const float CellWidth = 100f;

    private RectTransform _rect;


    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }


    public void UpdateCar(int carX)
    {
        _rect.anchoredPosition =
            new Vector2(
                carX * CellWidth,
                0
            );
    }
}