using UnityEngine;

public class PlayerIdleState : State
{
    private PlayerController player;

    public PlayerIdleState(PlayerController player)
    {
        this.player = player;
    }

    public override void Enter() { }

    public override void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        if (dir.magnitude > 0.1f)
        {
            player.Move(dir);
        }
    }

    public override void Exit() { }
}
