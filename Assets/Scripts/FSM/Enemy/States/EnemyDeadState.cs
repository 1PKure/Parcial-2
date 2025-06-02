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
        // L�gica al morir
    }

    public override void Update() { }

    public override void Exit() { }
}
