using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(Player _player){
        player = _player;
    }
    public override PlayerState HandleTransitions(){
        if (player.health <= 0) {
            return new DeadState(player);
        }
        if (player.inputDirection.x != 0f || player.inputDirection.y != 0f || !player.controller.isGrounded){
            return new NormalMovement(player);
        }
        if (player.inputs[4]){
            return new JumpState(player);
        }
        return null;
    }

    public override void StateUpdate(){
        player.HandleInput(player.inputSensitivity, player.inputGravity);
    }

    public override void Enter(){

    }

    public override void Exit(){

    }
}
