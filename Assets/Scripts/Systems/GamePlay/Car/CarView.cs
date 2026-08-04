using System.Collections;
using UnityEngine;

public class CarView : MonoBehaviour
{
    private const float CellWidth = 100f;

    private const float StunAlpha = 0.35f;
    private const float BlinkInterval = 0.3f;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;

    private Coroutine _stunCoroutine;


    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }


    public void UpdateCar(int carX, bool isStunned)
    {
        _rect.anchoredPosition =
            new Vector2(
                carX * CellWidth,
                0
            );

        UpdateStunState(isStunned);
    }


    private void UpdateStunState(bool isStunned)
    {
        if (isStunned)
        {
            if (_stunCoroutine == null)
                _stunCoroutine = StartCoroutine(StunRoutine());
        }
        else
        {
            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
                _stunCoroutine = null;
            }

            _canvasGroup.alpha = 1f;
        }
    }


    private IEnumerator StunRoutine()
    {
        while (true)
        {
            _canvasGroup.alpha = StunAlpha;
            yield return new WaitForSeconds(BlinkInterval);

            _canvasGroup.alpha = 1f;
            yield return new WaitForSeconds(BlinkInterval);
        }
    }
}