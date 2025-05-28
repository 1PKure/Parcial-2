public class EnemyDeadState : State
{
    private EnemyController enemy;

    public EnemyDeadState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        // L�gica al morir
    }

    public override void Update() { }

    public override void Exit() { }
}
