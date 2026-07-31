using UnityEngine;
using UnityEngine.UI;
using TMPro;
using block_racing_common.Game.Enums;
using block_racing_common.Game.Pieces;
using block_racing_common.Game.Snapshots;

public class PlayerUI : MonoBehaviour
{
    [Header("Block")]
    [SerializeField] private Transform nextBlockRoot;
    [SerializeField] private Image[] blockCells;

    [Header("Mode")]
    [SerializeField] private TMP_Text currentMode;

    [Header("Shoot Cooldown")]
    [SerializeField] private GameObject cooldownRoot;
    [SerializeField] private Image cooldownImage;

    private const float ShootCooldownTime = 1.5f;
    private const float CellSize = 50f;
    private const float CooldownRotationSpeed = 360f;

    public void UpdateUI(PlayerSnapshot snapshot)
    {
        if (snapshot == null)
        {
            Debug.LogError("PlayerSnapshot is null.");
            return;
        }

        UpdateCurrentPiece(
            snapshot.CurrentPieceType,
            snapshot.CurrentPieceRotation);

        UpdateMode(snapshot.Mode);

        UpdateCooldown(snapshot.ShootCooldownRemaining);
    }

    private void UpdateCurrentPiece(
        PieceType? type,
        Rotation? rotation)
    {
        // 현재 블록이 없는 상태
        if (!type.HasValue || !rotation.HasValue)
        {
            foreach (var cell in blockCells)
            {
                cell.enabled = false;
            }

            return;
        }

        CellPosition[] shape =
            PieceShapeTable.GetShape(
                type.Value,
                rotation.Value);

        for (int i = 0; i < blockCells.Length; i++)
        {
            if (i >= shape.Length)
            {
                blockCells[i].enabled = false;
                continue;
            }

            blockCells[i].enabled = true;

            RectTransform rect =
                blockCells[i].GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;

            rect.anchoredPosition =
                new Vector2(
                    shape[i].X * CellSize,
                    shape[i].Y * CellSize);
        }
    }

    private void UpdateMode(block_racing_common.Game.Enums.PlayMode mode)
    {
        switch (mode)
        {
            case block_racing_common.Game.Enums.PlayMode.Defense:
                currentMode.color = Color.blue;
                currentMode.text = "수비";
                break;
            case block_racing_common.Game.Enums.PlayMode.Attack:
                currentMode.color = Color.red;
                currentMode.text = "공격";
                break;
            default:
                currentMode.color = Color.black;
                currentMode.text = "?";
                break;
        }
    }

    private void UpdateCooldown(float remaining)
    {
        bool isCooldown =
            remaining > 0f;

        cooldownRoot.SetActive(isCooldown);

        if (!isCooldown)
            return;


        // 남은 쿨타임 비율
        cooldownImage.fillAmount =
            remaining / ShootCooldownTime;
    }
}