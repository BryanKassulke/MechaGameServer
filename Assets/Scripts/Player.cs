using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    public int id;
    public string username;
    public CharacterController controller;
    public Transform shootOrigin;
    public bool[] inputs;    
    public Vector2 inputDirection;
    public float gravity = -19.62f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 9f;
    public float inputSensitivity = 10f;
    public float inputGravity = 7f;
    public float airInputGravity = 1f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;

    //State
    private PlayerState state;

    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[5];

        state = new IdleState(this);
        state.Enter();
    }

    private void Start() {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
        inputSensitivity *= Time.fixedDeltaTime;
        inputGravity *= Time.fixedDeltaTime;
        airInputGravity *= Time.fixedDeltaTime;
    }

    public void FixedUpdate() {        
        HandleState(state.HandleTransitions());
    }

    public void HandleState(PlayerState _state){
        if (_state != null){
            state.Exit();
            state.Dispose();
            state = _state;
            state.Enter();
        }
        state.StateUpdate();
    }

    public void Move(Vector3 _moveDirection) {
        controller.Move(_moveDirection);
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation) {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void HandleInput(float _inputSensitivity, float _inputGravity){
        // Virtual Joystick X and Y
        if ((inputs[0] && inputs[1]) || (!inputs[0] && !inputs[1])){
            inputDirection.y = Mathf.MoveTowards(inputDirection.y, 0f, _inputGravity);
        } else {
            if (inputs[0]){
                if (inputDirection.y >= 0){
                    inputDirection.y = Mathf.MoveTowards(inputDirection.y, 1f, _inputSensitivity);
                } else {
                    inputDirection.y = Mathf.MoveTowards(inputDirection.y, 1f, _inputSensitivity + _inputGravity);
                }                
            }
            if (inputs[1]){
                if (inputDirection.y <= 0){
                    inputDirection.y = Mathf.MoveTowards(inputDirection.y, -1f, _inputSensitivity);
                } else {
                    inputDirection.y = Mathf.MoveTowards(inputDirection.y, -1f, _inputSensitivity + _inputGravity);
                }
            }
        }

        if ((inputs[2] && inputs[3]) || (!inputs[2] && !inputs[3])){
            inputDirection.x = Mathf.MoveTowards(inputDirection.x, 0f, _inputGravity);
        } else {
            if (inputs[2]){
                if (inputDirection.x >= 0){
                    inputDirection.x = Mathf.MoveTowards(inputDirection.x, -1f, _inputSensitivity);
                } else {
                    inputDirection.x = Mathf.MoveTowards(inputDirection.x, -1f, _inputSensitivity + _inputGravity);
                }                
            }
            if (inputs[3]){
                if (inputDirection.x <= 0){
                    inputDirection.x = Mathf.MoveTowards(inputDirection.x, 1f, _inputSensitivity);
                } else {
                    inputDirection.x = Mathf.MoveTowards(inputDirection.x, 1f, _inputSensitivity + _inputGravity);
                }
            }
        }
    }

    public void Shoot(Vector3 _viewDirection){
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f)){
            if (_hit.collider.CompareTag("Player")){
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    public void TakeDamage(float _damage){
        health -= _damage;
        if (health <= 0f){
            health = 0f;
        }
        ServerSend.PlayerHealth(this);
    }

    public bool AttemptPickupItem(){
        if (itemAmount >= maxItemAmount){
            return false;
        }

        itemAmount++;
        return true;
    }

}
