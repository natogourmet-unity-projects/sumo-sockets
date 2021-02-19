var shortID = require("shortid");
var Vector3 = require("./Vector3.js");

module.exports = class Player {
  constructor(_id) {
    this.nick = "";
    this.id = _id;
    this.isHost = false;
    this.isReady = false;
    this.position = new Vector3();
    this.rotation = new Vector3();
  }
};
