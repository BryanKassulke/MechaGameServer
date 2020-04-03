using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    private Vector3 moveDirection;    
    private float yVelocity;
    public JumpState(Player _player){
        player = _player;
    }
    public override PlayerState HandleTransitions(){
        if (yVelocity <= 0f){
            return new NormalMovement(player);
        }
        return null;
    }

    public override void StateUpdate(){
        player.HandleInput(player.inputSensitivity, player.airInputGravity);

        moveDirection = player.transform.right * player.inputDirection.x + player.transform.forward * player.inputDirection.y;
        if (moveDirection.magnitude > 1f){
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        }
        moveDirection *= player.moveSpeed;

        yVelocity += player.gravity;

        moveDirection.y = yVelocity;

        player.Move(moveDirection);
    }

    public override void Enter(){
        yVelocity = player.jumpSpeed;
    }

    public override void Exit(){

    }
}
