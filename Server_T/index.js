// var io = require("socket.io")(process.env.PORT || 3360);

const { stringify } = require("querystring");

var app = require("express")();
var server = require("http").Server(app);
var io = require("socket.io")(server);

server.listen(5000);

//Custom classes
var Player = require("./Classes/Player.js");

console.log("Server has started");

var pc = -1;
var players = [];
var sockets = [];
var playersLength = 0;
var ringScale = 20;
var playersReady = 0;
var loadedPlayers = 0;
var ringTimer;

io.on("connection", function (socket) {
  console.log("Connection Made!");

  pc++;
  var player = new Player("Player " + pc);
  var thisPlayerID = player.id;

  if (playersLength <= 0) {
    player.isHost = true;
    player.isReady = true;
  }

  players[thisPlayerID] = player;
  sockets[thisPlayerID] = socket;
  playersLength += 1;

  //El emit aquí le va a decir al cliente (Unity, el cual está escuchando) que realice estos eventos

  //Tell the client this is our ID for the server
  socket.emit("register", player);

  socket.on("setNickname", function (data) {
    player.nick = data.nick;
    for (var playerID in players) {
      if (playerID != thisPlayerID) {
        socket.emit("connectToLobby", players[playerID]);
      }
    }

    // Tells the player about its own connection
    socket.emit("connectToLobby", player);

    // Tells everyone about the player connection
    socket.broadcast.emit("connectToLobby", player);
  });

  // Tells the player about all the online players

  socket.on("sendReady", function (data) {
    player.isReady = data.isReady;
    if (data.isReady) {
      playersReady += 1;
    } else {
      playersReady -= 1;
    }
    console.log("rdy:" + playersReady);
    console.log("lenght:" + playersLength);

    socket.broadcast.emit("setReady", player);
  });

  socket.on("sendStart", function () {
    if (playersReady == playersLength - 1) {
      playersReady = 0;
      socket.broadcast.emit("loadGame", player);
      socket.emit("loadGame", player);
    }
  });

  socket.on("gameLoaded", function (data) {
    loadedPlayers += 1;
    ringScale = 20;
    if (loadedPlayers == playersLength) {
      loadedPlayers == 0;
      //Tell myself about everyone else in the game
      for (var playerID in players) {
        socket.emit("spawn", players[playerID]); //Nos dirá a nosotros sobre los demás
        socket.broadcast.emit("spawn", players[playerID]); //Nos dirá a nosotros sobre los demás
      }

      ringTimer = setInterval(() => {
        ringScale -= 0.2;
        socket.broadcast.emit("ringScaleDown", { rs: ringScale });
        socket.emit("ringScaleDown", { rs: ringScale });
        if (ringScale <= 2) {
          clearInterval(ringTimer);
        }
      }, 1000);
    }
  });

  //Positional Data from client
  socket.on("updatePosition", function (data) {
    player.position.x = data.position.x;
    player.position.y = data.position.y;
    player.position.z = data.position.z;

    player.rotation.x = data.rotation.x;
    player.rotation.y = data.rotation.y;
    player.rotation.z = data.rotation.z;

    socket.broadcast.emit("updatePosition", player); //Update my position to the other players. It's different from the first updatePosition
  });

  socket.on("sendHit", function (data) {
    socket.broadcast.emit("setHit", data);
  });

  //El servidor está escuchando a cuando se llama este evento
  socket.on("disconnect", function () {
    console.log("Player has disconnected");
    delete players[thisPlayerID];
    delete sockets[thisPlayerID];
    playersLength -= 1;

    if (playersLength == 0) {
      loadedPlayers = 0;
      playersReady = 0;
      clearInterval(ringTimer);
    }

    socket.broadcast.emit("disconnected", player);
  });
});
