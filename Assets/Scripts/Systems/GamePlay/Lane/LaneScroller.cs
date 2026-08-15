using UnityEngine;
using UnityEngine.UI;

public class LaneScroller : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;

    private RawImage _rawImage;
    private float scrollSpeed = 0f;

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        Scroll();
    }

    private void Scroll()
    {
        if (scrollSpeed <= 0f)
            return;

        Rect uv = _rawImage.uvRect;
        uv.y += scrollSpeed * Time.deltaTime;
        _rawImage.uvRect = uv;
    }

    public void SetScrollSpeed(float carSpeed)
    {
        scrollSpeed = carSpeed * speedMultiplier;
    }

    public void Stop()
    {
        scrollSpeed = 0f;
    }
}