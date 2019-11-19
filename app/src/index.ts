import "./css/aspstyles.css";
import "./css/main.css";
import {State} from "./State";
import { Connection } from "./Connection";
import * as signalR from "@aspnet/signalr";
import { Logger } from "./Logger";
import { renderInit } from "./render/render-init";
import { renderPost } from "./render/render-posts";
import { renderView } from "./render/render";
import { IPost } from "./ajaxMethods";
import { StatusEnum } from "./Ajax";
import { renderRooms } from "./render/render-rooms";



(async function init () {

    var connection = Connection(new signalR.HubConnectionBuilder().withUrl("/chatHub").build());

    connection.onStartRender(async rooms => {  
        console.log("callback!")
        renderInit(rooms);
        $("#reconnect-button").on("click", connection.restart);
        Logger.connectionState("Connectection state is on", StatusEnum.success);
        Logger.message("", StatusEnum.success);
    });
    
    connection.onReceiveRender((post : IPost) => {
        if (State.getActiveRoom() !== null && State.getActiveRoom().id == post.roomId) {  
            renderPost(post.userName, post.postBody);
        }
        Logger.message(`post message received in room: ${post.roomId}`, StatusEnum.success);
    });

    connection.onReceiveRoomRender((rooms) => {
        renderRooms(rooms);
        Logger.message(`new room added`, StatusEnum.success);
    });

    connection.onCloseRender(() => {
        renderView(false);
        Logger.connectionState("Connectection state is off", StatusEnum.fail);
    });

    connection.onRestartRender(() => {
        renderView(true);
        Logger.connectionState("Connectection state is on", StatusEnum.success);
        Logger.message("", StatusEnum.success);
    });
    
    await connection.start();
    console.log("dsds")

})();



