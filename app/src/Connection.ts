import { RoomsFactory } from "./Rooms";
import { State } from "./State";
import { getRooms } from "./ajaxMethods";


interface IHubConnection {
    onclose : (callback) => void;
    on : (name, callback) => void
    state : number
    start : () => void;
    send : (method : string, message: Object) => void;
}

export function Connection(connection : IHubConnection) {

    var appInit : Promise<boolean>;
    var logs = {};

    async function StartConnection (cb) : Promise<boolean> {
        var tries = 1;
        return (async function start() {
            try {
                await connection.start();
                await cb();
            } catch (error) {
                _internal.onStartFail(error);
                if (tries === 0) return false;
                tries--;
                start();
            }
        })();
    }
    
    function addLog(msg) {
        var datenow = new Date();
        logs[datenow.toString()] = msg;
    }
    const callback : any = {};

    const api = {
        onStart(cb) {
            callback['onStart'] = cb; 
        },
        onClose(cb) {
            callback['onClose'] = cb; 
        },
        onReceiveMessage(cb) {
            callback['onReceive'] = cb; 
        },
        onReceiveRoom(cb) {
            callback['onReceiveRoom'] = cb; 
        },
        onRestart(cb) {
            callback['onRestart'] = cb; 
        },
        onReceiveError(cb) {
            callback['onReceiveError'] = cb; 
        },
        async start() {
            await StartConnection(_internal.onStart);
        },
        async restart() {
            await StartConnection(_internal.onRestart);
         
        },
        send(message, roomId) {
           connection.send("SendMessage", {"message" : message, "roomId" : roomId});
        }
    }

    const _internal = {
        async onStart() {
            appInit = new Promise(async (res, rej) => {
                addLog("Connection started");
                await callback.onStart();
                res();
            });  
            await appInit;
        },
        onRestart() {
            callback.onRestart();
        },
        onStartFail(error) {
            addLog(error);
        },
        onclose() {
            addLog("Connection closed");
            callback.onClose();
        },
        async onReceiveMessage(userName, postBody, roomId, createDate) {
            addLog(`Message reveived from ${userName} in room: ${roomId}`);
            var post = {userName, postBody, roomId, createDate}
            // TODO : simplify - should just await onStart
            await appInit;
            callback.onReceive(post);
        },
        async onReceiveRoom(roomname, roomId) {
            addLog(`Room reveived with id: ${roomId}`);
            callback.onReceiveRoom({name : roomname, id : roomId});
        },
        async onReceiveError(error) {
            addLog(`Error : ${error}`);
            callback.onReceiveError(error);
        }
    }

    connection.onclose(_internal.onclose);
    connection.on("ReceiveMessage", _internal.onReceiveMessage);
    connection.on("ErrorMessage", _internal.onReceiveError);
    connection.on("CreateRoomMessage", _internal.onReceiveRoom);


    return api;

};
