using block_racing_common.Game.Snapshots;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LaneView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gridRoot;
    [SerializeField] private BlockView blockPrefab;

    [SerializeField] private Transform flyingRoot;
    [SerializeField] private FlyingBlockView flyingBlockPrefab;

    private const int Width = 5;
    private const int Height = 20;

    private BlockView[] _blocks;
    private List<FlyingBlockView> _flyingBlocks = new();

    private void Awake()
    {
        CacheBlocks();
    }


    [ContextMenu("Generate Grid")]
    private void GenerateGrid()
    {
        // 기존 Block 제거
        ClearGrid();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
#if UNITY_EDITOR
                BlockView block = (BlockView)PrefabUtility.InstantiatePrefab(blockPrefab, gridRoot);
#else
                BlockView block = Instantiate(blockPrefab, gridRoot);
#endif

                block.name = $"Block ({x},{y})";
            }
        }
    }

    [ContextMenu("Clear Grid")]
    private void ClearGrid()
    {
        for (int i = gridRoot.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(gridRoot.GetChild(i).gameObject);
#else
            Destroy(gridRoot.GetChild(i).gameObject);
#endif
        }
    }

    private void CacheBlocks()
    {
        _blocks = gridRoot.GetComponentsInChildren<BlockView>(true);

        if (_blocks.Length != Width * Height)
        {
            Debug.LogError(
                $"Expected {Width * Height} blocks, but found {_blocks.Length}.");
        }
    }

    public void UpdateLane(LaneSnapshot snapshot)
    {
        if (snapshot == null)
        {
            Debug.LogError("LaneSnapshot is null");
            return;
        }

        if (_blocks == null)
        {
            Debug.LogError("Block cache is null");
            return;
        }

        if (snapshot.Blocks.Length != Width * Height)
        {
            Debug.LogError(
                $"Invalid block size : {snapshot.Blocks.Length}");
            return;
        }

        if (_blocks.Length != snapshot.Blocks.Length)
        {
            Debug.LogError("Block count mismatch.");
            return;
        }

        for (int i = 0; i < snapshot.Blocks.Length; i++)
        {
            _blocks[i].SetBlock(snapshot.Blocks[i]);
        }

        UpdateFlyingBlocks(snapshot);
    }


    private void UpdateFlyingBlocks(LaneSnapshot snapshot)
    {
        var flyingSnapshots = snapshot.FlyingBlocks;


        // 필요한 만큼 생성
        while (_flyingBlocks.Count < flyingSnapshots.Count)
        {
            FlyingBlockView view =
                Instantiate(
                    flyingBlockPrefab,
                    flyingRoot);

            _flyingBlocks.Add(view);
        }


        // 활성 FlyingBlock 업데이트
        for (int i = 0; i < flyingSnapshots.Count; i++)
        {
            FlyingBlockView view =
                _flyingBlocks[i];

            view.gameObject.SetActive(true);

            view.UpdateBlock(
                flyingSnapshots[i]);
        }


        // 남는 View 비활성화
        for (int i = flyingSnapshots.Count;
             i < _flyingBlocks.Count;
             i++)
        {
            _flyingBlocks[i]
                .gameObject
                .SetActive(false);
        }
    }

}