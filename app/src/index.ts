import "./css/aspstyles.css";
import "./css/main.css";
import "./css/master-list.css";

import { Connection } from "./Connection";
import * as signalR from "@aspnet/signalr";
import { IPost, IRoom, getRooms } from "./ajaxMethods";
import { actionInit, actionInitRender } from "./actions/action-init";
import { actionReceiveMessage, actionReceiveMessageRender } from "./actions/action-receive-message";
import { actionReceiveRoom, actionReceiveRoomRender } from "./actions/action-receive-room";
import { actionOncloseRender } from "./actions/action-onclose";
import {  actionOnRestart } from "./actions/action-onrestart";
import { State } from "./State";
import { Logger } from "./GlobalLogger";
import { triggerInitRoom } from "./actions/action-room-select";


(async function init () {

    var connection = Connection(new signalR.HubConnectionBuilder().withUrl("/hub").build());

    connection
    .onStart(async () => {
        await actionInit(actionInitRender, triggerInitRoom)  
    })
    .onReceiveMessage(async (post : IPost) => {
        await actionReceiveMessage(post, actionReceiveMessageRender);   
    })
    .onReceiveRoom(async (room: IRoom) => {
        await actionReceiveRoom(room, actionReceiveRoomRender)
    })
    .onClose(async () => {
        await actionOncloseRender();
    })
    .onRestart(async () => {
        await actionOnRestart();
    })
    .onLog(async (log, status) => {
       if (log.trim().toLowerCase() == "fatalerror") {
        window.location.replace(location.origin + "/Home/Error");
       }
        Logger.message(log, status);
    })
    .onLogConnection(async (log, status) => {
        Logger.connectionState(log, status);
    });
    
    await connection.start();
    
    $("#reconnect-button").on("click", connection.restart);

    $("#sendButton").on("click", function (e) {
        var message = (<HTMLInputElement> document.getElementById("message-input")).value;
        const roomId = State.getActiveRoom().id;
        connection.send(message,roomId);
    });   
     
})();



