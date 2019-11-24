import "./css/aspstyles.css";
import "./css/main.css";

import { Connection } from "./Connection";
import * as signalR from "@aspnet/signalr";
import { IPost, IRoom } from "./ajaxMethods";
import { actionInit, actionInitRender } from "./actions/action-init";
import { actionReceiveMessage, actionReceiveMessageRender } from "./actions/action-receive-message";
import { actionReceiveRoom, actionReceiveRoomRender } from "./actions/action-receive-room";
import { actionOncloseRender } from "./actions/action-onclose";
import { actionOnRestartRender, actionOnRestart } from "./actions/action-onrestart";
import { State } from "./State";
import { Logger } from "./GlobalLogger";


(async function init () {

    var connection = Connection(new signalR.HubConnectionBuilder().withUrl("/hub").build());

    connection.onStart(async () => {
        await actionInit(actionInitRender)  
    });
    
    connection.onReceiveMessage(async (post : IPost) => {
        await actionReceiveMessage(post, actionReceiveMessageRender);   
    });

    connection.onReceiveRoom(async (room: IRoom) => {
        await actionReceiveRoom(room, actionReceiveRoomRender)
    });

    connection.onClose(async () => {
        await actionOncloseRender();
    });

    connection.onRestart(async () => {
        await actionOnRestart();
    });

    connection.onLog(async (log, status) => {
       if (log === "Error") {
        window.location.replace(location.origin + "/Home/Error");
       }
        Logger.message(log, status);
    });

    connection.onLogConnection(async (log, status) => {
        Logger.connectionState(log, status);
    });
    
    await connection.start();
    
    $("#reconnect-button").on("click", connection.restart);

    $("#sendButton").on("click", function (e) {
        var message = (<HTMLInputElement> document.getElementById("messageInput")).value;
        const roomId = State.getActiveRoom().id;
        connection.send(message,roomId);
    });    
})();



