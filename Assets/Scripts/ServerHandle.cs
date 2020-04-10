using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet) {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck) {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID: ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerInput(int _fromClient, Packet _packet) {      
        InputMessage _inputMessage;
        _inputMessage.tickNumber = _packet.ReadUInt();
        _inputMessage.inputs.timeStep = _packet.ReadFloat();
        _inputMessage.inputs.w = _packet.ReadBool();
        _inputMessage.inputs.s = _packet.ReadBool();
        _inputMessage.inputs.a = _packet.ReadBool();
        _inputMessage.inputs.d = _packet.ReadBool();
        _inputMessage.inputs.space = _packet.ReadBool();
        _inputMessage.inputs.camOffset = _packet.ReadVector3();
        _inputMessage.inputs.lookDir = _packet.ReadVector3();
        
        Server.clients[_fromClient].player.AddToInputQueue(_inputMessage);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet){
        Vector3 _camPosition = _packet.ReadVector3();
        Vector3 _lookDir = _packet.ReadVector3();
        
        Server.clients[_fromClient].player.Shoot(_camPosition, _lookDir);
    }
}
