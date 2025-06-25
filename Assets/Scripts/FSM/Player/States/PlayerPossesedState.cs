using UnityEngine;

public class PlayerPossessedState : State
{
    public override StateType StateType => StateType.Possessed;

    private PlayerController2 player;

    public PlayerPossessedState(PlayerController2 player) : base(player.gameObject, player.GetStateMachine())
    {
        this.player = player;
    }

    public override void Enter()
    {
        UIManager.Instance.ShowMessage("Entered Possessed State ? Player inerte");
    }

    public override void Update()
    {
        // No hace nada ? Player queda inerte
    }

    public override void Exit()
    {
        UIManager.Instance.ShowMessage("Exited Possessed State ? Player vuelve a funcionar");
    }
}
