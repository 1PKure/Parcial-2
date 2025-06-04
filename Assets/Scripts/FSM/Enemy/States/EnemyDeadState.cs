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
        // L�gica al morir
    }

    public override void Update() { }

    public override void Exit() { }
}
