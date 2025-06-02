public class EnemyDeadState : State
{
    private EnemyController enemy;

    public EnemyDeadState(EnemyController enemyController)
        : base(enemyController.gameObject, enemyController.GetStateMachine())
    {
        enemy = enemyController;
    }

    public override void Enter()
    {
        // Lógica al morir
    }

    public override void Update() { }

    public override void Exit() { }
}
