using UnityEngine;

namespace Clase10
{
    public class PlayerPossessedState : State
    {
        private PlayerController2 player;

        public PlayerPossessedState(PlayerController2 player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            Debug.Log("Entered Possessed State ? Player inerte");
        }

        public override void Update()
        {
            // No hace nada ? Player queda inerte
        }

        public override void Exit()
        {
            Debug.Log("Exited Possessed State ? Player vuelve a funcionar");
        }
    }
}
