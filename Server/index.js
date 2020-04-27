const express = require('express');
const socketIO = require('socket.io');
const path = require('path');
const Player = require('./Player');
const NetworkCommand = require('./function');
const Room = require('./Room');

const PORT = process.env.PORT || 8888;
const INDEX = path.join(__dirname, 'index.html');

const server = express()
    .use((req, res) => res.sendFile(INDEX))
    .listen(PORT, () => console.log(`Listening on ${PORT}`));

const io = socketIO(server);

console.log('server has started');

var players = [];
var sockets = [];
var rooms = [];
var waitingPlayers = [];

io.on('connection', (socket) => {
    console.log('Have a player connected');
    var player = new Player();
    var playerID = player.id;

    players[playerID] = player;
    sockets[playerID] = socket;

    // Return id to player
    socket.emit(NetworkCommand.register, { id: playerID });

    // Detech player register name
    socket.on(NetworkCommand.registerName, (data) => {
        var name = data.name;
        player.userName = name;
        console.log('register name: %s', name);
    });

    socket.on('disconnect', () => {
        HandleExitRoom(socket, playerID);

        // Delete player in list
        delete players[playerID];
        delete sockets[playerID];
        delete waitingPlayers[playerID];

        console.log('Rooms: ' + rooms.length);
        console.log('disconnected');
    })

    socket.on(NetworkCommand.updatePosition, (data) => {
            PassData(socket,data,NetworkCommand.updatePosition);
    });

    socket.on(NetworkCommand.updateBallPosition, (data) => {
        PassData(socket,data,NetworkCommand.updateBallPosition);
    });

    socket.on(NetworkCommand.updateVelo, (data) => {
        PassData(socket,data,NetworkCommand.updateVelo);
    });

    socket.on(NetworkCommand.fireBall, (data) => {
        PassData(socket,data,NetworkCommand.fireBall);
    });

    socket.on(NetworkCommand.updateArrowRotation, (data) => {
        PassData(socket,data,NetworkCommand.updateArrowRotation);
    });

    socket.on(NetworkCommand.hasWin,()=>{
        var roomHaveSocket = GetRoomHaveSocket(socket);
        roomHaveSocket.HasWin(socket);
    });

    socket.on(NetworkCommand.startMatchMaking, () => {
        waitingPlayers[playerID] = player;
        var numsPlayerWaiting = Object.keys(waitingPlayers).length;

        console.log('players waiting: %i', numsPlayerWaiting);
        if (numsPlayerWaiting >= 2) {
            var room = new Room();

            rooms[room.id] = room;
            // Delete room callback
            room.deleteRoomCallBack = () => {
                console.log("Delete room");
                delete rooms[room.id];
            }
            room.player1 = player;
            room.player1.index = 1;

            // Find another player
            for (var key in waitingPlayers) {
                var value = waitingPlayers[key];
                if (value.id != playerID) {
                    room.player2 = value;
                    room.player2.index = 2;
                    break;
                }
            }

            // Bind socket to room
            room.socket1 = socket;
            room.socket2 = sockets[room.player2.id];

            // Delete in waiting list
            delete waitingPlayers[room.player1.id];
            delete waitingPlayers[room.player2.id];

            // Emit start game on 2 player
            console.log(room.player1.userName);
            console.log(room.player2.userName);
            room.socket1.emit(NetworkCommand.startGame, room.player2);
            room.socket2.emit(NetworkCommand.startGame, room.player1);
        }
    })

});

function HandleExitRoom(socket, playerID) {

    // Delete room if have a player quit
    var roomHaveSocket = GetRoomHaveSocket(socket);
    if (roomHaveSocket != null) {
        // Send win to the other player
        // if (roomHaveSocket.player1.id == playerID) {
        //     roomHaveSocket.setPlayerWin(2);
        // } else {
        //     roomHaveSocket.setPlayerWin(1);
        // }

        roomHaveSocket.deleteRoom();
    }
}

function PassData(socket, data, command)
{
    var roomValue = GetRoomHaveSocket(socket);
    if (roomValue) {
        var opponentSocket = roomValue.getOpponentSocket(socket);
        if (opponentSocket != null)
            opponentSocket.emit(command, data);
    }
}

function GetRoomHaveSocket(socket) {
    for (var room in rooms) {
        var roomValue = rooms[room];
        if (roomValue.hasSocket(socket)) {
            return roomValue;
        }
    }
    return null;
}