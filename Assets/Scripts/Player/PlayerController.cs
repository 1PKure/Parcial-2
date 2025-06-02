using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private StateMachine stateMachine;

    void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        stateMachine.Update();
    }

    public StateMachine GetStateMachine()
    {
        return stateMachine;
    }

    public void Move(Vector3 direction)
    {
        transform.Translate(direction * Time.deltaTime * 5f);
    }
}
