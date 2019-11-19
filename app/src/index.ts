import "./css/aspstyles.css";
import "./css/main.css";

import { Connection } from "./Connection";
import * as signalR from "@aspnet/signalr";
import { Logger } from "./Logger";

import { renderView } from "./render/render";
import { IPost, IRoom } from "./ajaxMethods";
import { StatusEnum } from "./Ajax";

import { actionInit, actionInitRender } from "./actions/action-init";
import { actionReceiveMessage, actionReceiveMessageRender } from "./actions/action-receive-message";
import { actionReceiveRoom, actionReceiveRoomRender } from "./actions/action-receive-room";
import { actionOncloseRender } from "./actions/action-onclose";
import { actionOnRestartRender } from "./actions/action-onrestart";



(async function init () {

    var connection = Connection(new signalR.HubConnectionBuilder().withUrl("/chatHub").build());

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
        await actionOnRestartRender();
    });
    
    await connection.start();
    
    $("#reconnect-button").on("click", connection.restart);
})();



