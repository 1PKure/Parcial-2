public class EnemyIdleState : State
{
    private EnemyController enemy;

    public EnemyIdleState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter() { }

    public override void Update()
    {
        // Lógica para cambiar a patrulla o persecución
    }

    public override void Exit() { }
}
