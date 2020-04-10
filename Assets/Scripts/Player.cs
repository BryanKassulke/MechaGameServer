using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Inputs{    
    public float timeStep;
    public bool w;
    public bool s;
    public bool a;
    public bool d;
    public bool space;
    public Vector3 camOffset;
    public Vector3 lookDir;
}

public struct InputMessage{
    public uint tickNumber;
    public Inputs inputs;
}

public struct ClientState{
    public Vector3 position;
    public Quaternion rotation;
}

public struct StateMessage{
    public uint tickNumber;
    public Vector3 position;
    public Quaternion rotation;
}


public class Player : MonoBehaviour{
    public int id;
    public string username;
    public CharacterController controller;
    public Transform shootOrigin;
    public Transform camTransform;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    public float moveSpeed = 5f;

    // Server logic state
    private Queue<InputMessage> serverInputMessages;
    private uint serverTick;
    private uint serverTickAccumulator;
    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    private void Start() {
        serverInputMessages = new Queue<InputMessage>();
        serverTick = 0;
        serverTickAccumulator = 0;
    }

    private void FixedUpdate() {
        uint _serverTick = serverTick;
        uint _serverTickAccumulator = serverTickAccumulator;
        // 1. Handle Inputs in order than they arrive
        while (serverInputMessages.Count > 0){
            InputMessage _inputMessage = serverInputMessages.Dequeue();
            
            uint _tick = _inputMessage.tickNumber;
            // If the input tick is greater or equal to ther servers, then it has new input
            if (_tick >= serverTick){
                // Perform the input on the player
                Move(_inputMessage.inputs);
                camTransform.position = transform.position + _inputMessage.inputs.camOffset;
                
                ++_serverTick;
                // 2. Send that shit off!!
                StateMessage _stateMessage;
                _stateMessage.tickNumber = _serverTick;
                _stateMessage.position = transform.position;
                _stateMessage.rotation = transform.rotation;
                ServerSend.PlayerState(this, _stateMessage);

                //this.server_display_player.transform.position = server_rigidbody.position;
                //this.server_display_player.transform.rotation = server_rigidbody.rotation;
            }            
        }
        serverTick = _serverTick;
    }

    private void Move(Inputs _inputs){
        Quaternion _newRotation = Quaternion.LookRotation(_inputs.lookDir);
        _newRotation = Quaternion.Euler(0, _newRotation.eulerAngles.y, 0);
        transform.rotation = _newRotation;

        Vector2 _inputDirection = Vector2.zero;
        if (_inputs.w) {
            _inputDirection.y += 1;
        }
        if (_inputs.s) {
            _inputDirection.y -= 1;
        }
        if (_inputs.a) {
            _inputDirection.x -= 1;
        }
        if (_inputs.d) {
            _inputDirection.x += 1;
        }
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        if (_moveDirection.magnitude > 1f){
            _moveDirection = Vector3.ClampMagnitude(_moveDirection, 1f);
        }
        _moveDirection *= moveSpeed;
        _moveDirection *= _inputs.timeStep;

        //transform.Translate(_moveDirection);
        controller.Move(_moveDirection);
    }
    

    public void AddToInputQueue(InputMessage _inputMessage) {
        serverInputMessages.Enqueue(_inputMessage);
    }

    public void Shoot(){
        Vector3 _aimDirection = camTransform.forward;
        //TODO: Find a solution that requires less raycasts and feels less snappy
        if (Physics.Raycast(camTransform.position, _aimDirection, out RaycastHit _preHit, 100f)){
            Vector3 _shootDirection = (_preHit.point - shootOrigin.position).normalized;
            if (Physics.Raycast(shootOrigin.position, _shootDirection, out RaycastHit _hit, 100f)){
                if (_hit.collider.CompareTag("Player")){
                    _hit.collider.GetComponent<Player>().TakeDamage(10f);
                }
                ServerSend.SpawnBullet(shootOrigin.position, _hit.point);
            }
            ServerSend.SpawnBullet(shootOrigin.position, _preHit.point);
        } else {
            Vector3 _direction = (camTransform.position + (_aimDirection * 100f)) - shootOrigin.position;
            if (Physics.Raycast(shootOrigin.position, _direction, out RaycastHit _hit2, 100f)){
                ServerSend.SpawnBullet(shootOrigin.position, _hit2.point);
            } else {
                ServerSend.SpawnBullet(shootOrigin.position, camTransform.position + (_aimDirection * 100f));
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
