
public class EnemyDeadState : State
{
    public override StateType StateType => StateType.Dead;

    private EnemyController enemy;

    public EnemyDeadState(EnemyController enemy) : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
    }

    public override void Enter() { }
    public override void Update() { }
    public override void Exit() { }
}
