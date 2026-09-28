using UnityEngine;
using KBCore.Refs;

namespace Platformer {
    public class PushState : BaseState {
        public PushState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            if (!player.MovementEnabled)
                return;
            animator.CrossFade(PushHash, crossFadeDuration);
        }

        public override void FixedUpdate() {
            //player.HandlePush();
            player.HandleMovement();
        }
    }
}
