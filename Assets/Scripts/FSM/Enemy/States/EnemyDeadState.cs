public class EnemyDeadState : State
{
    private EnemyController enemy;

    public EnemyDeadState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        // Lógica al morir
    }

    public override void Update() { }

    public override void Exit() { }
}
