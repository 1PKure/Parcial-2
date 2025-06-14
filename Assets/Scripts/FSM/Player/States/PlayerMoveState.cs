using UnityEngine;

    public class PlayerMoveState : State
    {
        private PlayerController2 player;

        public PlayerMoveState(PlayerController2 player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            Debug.Log("Entered Move State");
        }

        public override void Update()
        {
            Vector3 inputDir = player.GetInputDirection();

            if (inputDir.magnitude <= 0.1f)
            {
                player.ChangeState(new PlayerIdleState(player));
                return;
            }

            player.Move(inputDir);
        }

        public override void Exit()
        {
            Debug.Log("Exited Move State");
        }
    }
