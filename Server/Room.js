const shortID = require('shortid');
const Player = require('./Player');
const NetworkCommand = require('./NetworkCommand');

module.exports = class Room {
    constructor() {
        this.player1 = null;
        this.player2 = null;
        this.socket1 = null;
        this.socket2 = null;
        this.id = shortID.generate();
        this.timer = 120;
        this.player1Score = 0;
        this.player2Score = 0;
        this.playerOutOfMoveFirst = null;


        // setTimeout(() => {
        //     console.log("Time out!");
        // }, 60000);
        this.interValID = setInterval(() => {
            if (this.socket1 != null)
                this.socket1.emit(NetworkCommand.updateTimer, { time: this.timer });
            if (this.socket2 != null)
                this.socket2.emit(NetworkCommand.updateTimer, { time: this.timer });
            this.timer--;
            if(this.timer == -1)
            {
                this.onEndGame();
            }
        }, 1000);
    }
    onEndGame()
    {
        // -1 -> LOSE
        // 0 -> DRAW
        // 1 -> Win
        if (this.player1Score > this.player2Score)
        {
            this.setPlayerWin(1);
        }
        else if(this.player2Score > this.player1Score)
        {
            this.setPlayerWin(2);
        }
        else
        {
            this.setPlayerWin(0);
        }
    }
    deleteRoom()
    {
        clearInterval(this.interValID);
        this.deleteRoomCallBack();
    }
    deleteRoomCallBack()
    {

    }

    hasSocket(socket) {
        return this.socket1 == socket || this.socket2 == socket;
    }
    setPlayerWin(player)
    {
        if (player == 1)
        {
            console.log("Player1 win");
            this.socket1.emit(NetworkCommand.endGame,{result: 1});
            this.socket2.emit(NetworkCommand.endGame,{result: -1});
        }
        else if(player == 2)
        {
            console.log("Player2 win");
            this.socket2.emit(NetworkCommand.endGame,{result: 1});
            this.socket1.emit(NetworkCommand.endGame,{result: -1});
        }
        else
        {
            console.log("Draw");
            this.socket1.emit(NetworkCommand.endGame,{result: 0});
            this.socket2.emit(NetworkCommand.endGame,{result: 0});
        }
        this.deleteRoom();
    }

    getOpponentSocket(socket) {
        if (this.socket1 == socket)
            return this.socket2;
        else return this.socket1;
    }
    
    handleOutOfMove(socket)
    {
        // Flow handle
        // If out of move and opponent has higher score => opponent win
        // else waiting until opponent get higher score or time end
        if (socket == this.socket1)
        {
            if (this.player1Score < this.player2Score)
            {
                this.setPlayerWin(2);
            }
            else
            {
                if (this.playerOutOfMoveFirst == this.player2)
                {
                    this.setPlayerWin(1);
                }
            }
            this.playerOutOfMoveFirst = this.player1;
        }
        else
        {
            if(this.player2Score < this.player1Score)
            {
                this.setPlayerWin(1);
            }
            else
            {
                if (this.playerOutOfMoveFirst == this.player1)
                {
                    this.setPlayerWin(2);
                }
            }
            this.playerOutOfMoveFirst = this.player2;
        }
    }

    updateScore(socket,score)
    {
        if (socket == this.socket1)
        {
            this.player1Score = Math.floor(score);
            // If have score > opponent and opponent is out of move -> win
            if (this.playerOutOfMoveFirst == this.player2 && this.player1Score > this.player2Score)
            {
                console.log("player1 score: "+ this.player1Score + "player2 score: " + this.player2Score);
                this.setPlayerWin(1);
            }
        }
        else
        {
            this.player2Score = Math.floor(score);
            if (this.playerOutOfMoveFirst == this.player1 && this.player2Score > this.player1Score)
            {
                console.log("player1 score: "+ this.player1Score + "player2 score: " + this.player2Score);
                this.setPlayerWin(2);
            }
        }
    }

}