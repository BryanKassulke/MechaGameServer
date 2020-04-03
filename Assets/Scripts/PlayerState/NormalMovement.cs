using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMovement : PlayerState
{
    private Vector3 moveDirection;
    private float yVelocity;
    public NormalMovement(Player _player){
        player = _player;
    }
    public override PlayerState HandleTransitions(){
        if (player.health <= 0) {
            return new DeadState(player);
        }
        if (player.inputDirection.x == 0f && player.inputDirection.y == 0f && player.controller.isGrounded){
            return new IdleState(player);
        }
        if (player.inputs[4] && player.controller.isGrounded){
            return new JumpState(player);
        }
        return null;
    }

    public override void StateUpdate(){
        if (player.controller.isGrounded){
            player.HandleInput(player.inputSensitivity, player.inputGravity);

            yVelocity = 0f;
        } else {
            player.HandleInput(player.inputSensitivity, player.airInputGravity);

             yVelocity += player.gravity;
        }
        
        moveDirection = player.transform.right * player.inputDirection.x + player.transform.forward * player.inputDirection.y;
        if (moveDirection.magnitude > 1f){
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        }
        moveDirection *= player.moveSpeed;  

        moveDirection.y = yVelocity;

        player.Move(moveDirection);
    }

    public override void Enter(){

    }

    public override void Exit(){

    }

}
