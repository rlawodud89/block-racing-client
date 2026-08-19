using System.Collections;
using TMPro;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    public static WarningUI Instance { get; private set; }

    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private float displayTime = 2f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    public void Hide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        warningPanel.SetActive(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);

        warningPanel.SetActive(false);
        hideCoroutine = null;
    }
}