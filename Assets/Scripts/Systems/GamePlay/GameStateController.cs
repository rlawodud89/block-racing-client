using System.Linq;
using UnityEngine;
using block_racing_common.Game.Snapshots;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    [SerializeField] private LaneView myLaneView;
    [SerializeField] private LaneView opponentLaneView;

    [SerializeField] private CarView myCarView;
    [SerializeField] private CarView opponentCarView;

    [SerializeField] private PlayerUI myPlayerUI;

    private long _lastTick = -1;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplySnapshot(GameStateSnapshot snapshot)
    {
        // 오래된 스냅샷은 무시
        if (snapshot.Tick <= _lastTick)
            return;

        _lastTick = snapshot.Tick;

        int myId = ClientContext.PlayerId;

        PlayerSnapshot mySnapshot = null;
        PlayerSnapshot opponentSnapshot = null;

        foreach (var player in snapshot.Players)
        {
            if (player.Id == myId)
                mySnapshot = player;
            else
                opponentSnapshot = player;
        }

        if (mySnapshot == null)
        {
            Debug.LogError($"My snapshot not found. MyId={myId}");
            return;
        }

        if (opponentSnapshot == null)
        {
            Debug.LogError("Opponent snapshot not found");
            return;
        }

        // 내 Lane은 항상 왼쪽
        myLaneView.UpdateLane(mySnapshot.Lane);

        // 상대 Lane은 항상 오른쪽
        opponentLaneView.UpdateLane(opponentSnapshot.Lane);

        // 차 위치
        myCarView.UpdateCar(mySnapshot.CarX);
        opponentCarView.UpdateCar(opponentSnapshot.CarX);

        //// 내 모드 UI
        myPlayerUI.UpdateUI(mySnapshot);
    }
}