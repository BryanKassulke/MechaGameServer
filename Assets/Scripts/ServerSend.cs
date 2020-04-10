using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend {
    private static void SendTCPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            if (i != _exceptClient) {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    private static void SendUDPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendUDPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            if (i != _exceptClient) {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    public static void Welcome(int _toClient, string _msg) {
        using (Packet _packet = new Packet((int)ServerPackets.welcome)) {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player) {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer)) {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }
    
    public static void PlayerState(Player _player, StateMessage _stateMessage) {
        using (Packet _packet = new Packet((int)ServerPackets.playerState)) {
            _packet.Write(_player.id);
            _packet.Write(_stateMessage.tickNumber);
            _packet.Write(_stateMessage.position);
            _packet.Write(_stateMessage.rotation);

            SendUDPData(_player.id, _packet);

            PlayerPosition(_player, _stateMessage.position);
            PlayerRotation(_player, _stateMessage.rotation);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    private static void PlayerPosition(Player _player, Vector3 _position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_position);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    private static void PlayerRotation(Player _player, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId) {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected)) {
            _packet.Write(_playerId);
            
            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player){
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth)) {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRespawn(Player _player){
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned)) {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void CreateItemSpawner(int _toClient, int _spawnerId, Vector3 _spawnerPosition, bool _hasItem){
        using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner)) {
            _packet.Write(_spawnerId);
            _packet.Write(_spawnerPosition);
            _packet.Write(_hasItem);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void ItemSpawned(int _spawnerId){
        using (Packet _packet = new Packet((int)ServerPackets.itemSpawned)) {
            _packet.Write(_spawnerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ItemPickedUp(int _spawnerId, int _byPlayer){
        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp)) {
            _packet.Write(_spawnerId);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnBullet(Vector3 _origin, Vector3 _destination){
        using (Packet _packet = new Packet((int)ServerPackets.spawnBullet)) {
            _packet.Write(_origin);
            _packet.Write(_destination);

            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}
