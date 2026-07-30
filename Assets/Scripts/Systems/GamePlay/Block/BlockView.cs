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
        image.enabled = data != 0;

        // TODO :
        // blockData를 이용해서
        // 색상이나 Sprite 변경
    }
}