using UnityEngine;

public class FinishLineView : MonoBehaviour
{
    private const int LaneHeight = 20;
    private const float CellHeight = 55f;

    // 화면 하나(Lane)를 이동하는 데 필요한 게임 거리
    private const float DistancePerLane = 15f;

    private RectTransform _rect;


    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }


    public void UpdateFinishLine(float currentDistance, float targetDistance)
    {
        float remainingDistance = targetDistance - currentDistance;

        // 아직 결승선이 화면에 들어오지 않음
        if (remainingDistance > DistancePerLane)
        {
            gameObject.SetActive(false);
            return;
        }


        // 결승선에 도착 이후
        if (remainingDistance < 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // 0 → 1
        // 결승선이 화면 위쪽에서 아래쪽으로 내려오는 진행도
        float progress = 1f - (remainingDistance / DistancePerLane);


        // Lane 위쪽 → Car 위치
        float startY = (LaneHeight - 1) * CellHeight;

        float endY = 0f;


        float y = Mathf.Lerp(
                startY,
                endY,
                progress);


        _rect.anchoredPosition = new Vector2(0f, y);
    }
}