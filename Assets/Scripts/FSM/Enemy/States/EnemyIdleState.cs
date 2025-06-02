public class EnemyIdleState : State
{
    private EnemyController enemy;

    public EnemyIdleState(EnemyController enemyController)
        : base(enemyController.gameObject, enemyController.GetStateMachine())
    {
        enemy = enemyController;
    }

    public override void Enter() { }

    public override void Update()
    {
        // L�gica para cambiar a patrulla o persecuci�n
    }

    public override void Exit() { }
}
