using UnityEngine;

public class PlayerIdleState : State
{
    public override StateType StateType => StateType.Idle;

    private PlayerController2 player;

    public PlayerIdleState(PlayerController2 player) : base(player.gameObject, player.GetStateMachine())
    {
        this.player = player;
    }

    public override void Enter() { }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = player.GetInputDirection();
        if (dir.magnitude > 0.1f)
        {
            player.ChangeState(StateType.Move);
        }
    }

    public override void Exit() { }
}
