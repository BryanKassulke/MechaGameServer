using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    private bool respawn;
    private float enterTime;
    public DeadState(Player _player){
        player = _player;
    }
    public override PlayerState HandleTransitions(){
        if (respawn){
            return new NormalMovement(player);
        }
        return null;
    }

    public override void StateUpdate(){
        player.HandleInput(player.inputSensitivity, player.inputGravity);
        if (Time.time >= enterTime + 5f){
            respawn = true;
        }
    }

    public override void Enter(){
        player.controller.enabled = false;
        player.transform.position = new Vector3(0f, 25f, 0f);
        
        respawn = false;  
        enterTime = Time.time;

        ServerSend.PlayerPosition(player);
    }

    public override void Exit(){
        player.health = player.maxHealth;
        player.controller.enabled = true;

        ServerSend.PlayerRespawn(player);
    }
}
