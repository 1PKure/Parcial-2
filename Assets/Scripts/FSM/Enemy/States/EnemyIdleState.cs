using Clase10;

public class EnemyIdleState : State
{
    private EnemyController enemy;

    public EnemyIdleState(EnemyController enemyController)
    {
        enemy = enemyController;
    }

    public override void Enter() { }

    public override void Update()
    {
        // Lógica para cambiar a patrulla o persecución
    }

    public override void Exit() { }
}
