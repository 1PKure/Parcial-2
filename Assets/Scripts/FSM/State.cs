using UnityEngine;

public abstract class State
{
    public abstract StateType StateType { get; }

    protected GameObject owner;
    protected StateMachine stateMachine;

    protected State(GameObject owner, StateMachine stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}