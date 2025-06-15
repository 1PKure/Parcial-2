
    public class StateMachine
    {
        private State currentState;

        public void Initialize(State startingState)
        {
            currentState = startingState;
            currentState.Enter();
        }

        public void ChangeState(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }
    }
