using UnityEngine;

public class PlayerMoveState : State
{
    public override StateType StateType => StateType.Move;

    private PlayerController2 player;

    public PlayerMoveState(PlayerController2 player) : base(player.gameObject, player.GetStateMachine())
    {
        this.player = player;
    }

    public override void Enter() { }

    public override void Update()
    {
        Vector3 dir = player.GetInputDirection();
        if (dir.magnitude <= 0.1f)
        {
            player.ChangeState(StateType.Idle);
            return;
        }

        player.Move(dir);
    }

    public override void Exit() { }
}
