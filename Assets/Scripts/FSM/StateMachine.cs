using System.Collections.Generic;

public class StateMachine
{
    private State currentState;
    private List<State> states = new List<State>();

    public void AddState(State state)
    {
        states.Add(state);
    }

    public void ChangeState(StateType type)
    {
        currentState?.Exit();

        currentState = states.Find(s => s.StateType == type);
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}

