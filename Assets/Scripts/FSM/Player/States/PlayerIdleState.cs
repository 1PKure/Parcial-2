using UnityEngine;

    public class PlayerIdleState : State
    {
        private PlayerController2 player;

        public PlayerIdleState(PlayerController2 player)
        {
            this.player = player;
        }

        public override void Enter()
        {
            Debug.Log("Entered Idle State");
        }

        public override void Update()
        {
            Vector3 inputDir = player.GetInputDirection();

            if (inputDir.magnitude > 0.1f)
            {
                player.ChangeState(new PlayerMoveState(player));
            }
        }

        public override void Exit()
        {
            Debug.Log("Exited Idle State");
        }
    }
