using Clase10;

public class EnemyDeadState : State
{
    private EnemyController enemy;

    public EnemyDeadState(EnemyController enemyController)
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
