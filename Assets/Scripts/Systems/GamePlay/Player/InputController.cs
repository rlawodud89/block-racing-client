using block_racing_common.Game.Enums;
using block_racing_common.Network.Packets;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private bool _canInput = false;

    private void Awake()
    {
        GameStartSequenceController.OnGameStarted += EnableInput;
    }

    private void OnDestroy()
    {
        GameStartSequenceController.OnGameStarted -= EnableInput;
    }

    private void EnableInput()
    {
        _canInput = true;
        Debug.Log("Input Enabled");
    }

    private void Update()
    {
        if (!_canInput)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SendInput(InputType.MoveLeft);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SendInput(InputType.MoveRight);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendInput(InputType.Shoot);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SendInput(InputType.Rotate);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SendInput(InputType.ChangeMode);
        }
    }

    private void SendInput(InputType type)
    {
        var packet = new C_InputPacket
        {
            InputType = type
        };

        NetworkManager.Instance.SendAsync(packet);
    }
}