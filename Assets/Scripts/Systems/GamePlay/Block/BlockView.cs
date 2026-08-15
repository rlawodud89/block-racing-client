using UnityEngine;
using UnityEngine.UI;

public class BlockView : MonoBehaviour
{
    [SerializeField] private Image image;

    private void Start()
    {
        SetBlock(0);
    }

    public void SetBlock(byte data)
    {
        if (image == null)
        {
            Debug.LogWarning(
                $"[BlockView] Image is already destroyed. Object={gameObject.name}"
            );

            return;
        }

        image.enabled = data != 0;
    }
}