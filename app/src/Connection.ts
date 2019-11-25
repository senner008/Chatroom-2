
import { StatusEnum } from "./Ajax";


interface IHubConnection {
    onclose : (callback) => void;
    on : (name, callback) => void
    state : number
    start : () => void;
    send : (method : string, message: Object) => void;
}

export function Connection(connection : IHubConnection) {

    var appInit : Promise<boolean>;

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
    
    function addLog(msg, status) {
        callback.onLog(msg, status);
    }
    function addLogConnection(msg, status) {
        callback.onLogConnection(msg, status);
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
        async start() {
            await StartConnection(_internal.onStart);
        },
        async restart() {
            await StartConnection(_internal.onRestart);
         
        },
        send(message, roomId) {
           connection.send("SendMessage", {"message" : message, "roomId" : roomId});
        },
        onLog(cb) {
            callback['onLog'] = cb;
        },
        onLogConnection(cb) {
            callback['onLogConnection'] = cb;
        }
    }

    const _internal = {
        async onStart() {
            appInit = new Promise(async (res, rej) => {
                await callback.onStart();
                addLogConnection("Connection started", StatusEnum.success);
                res();
            });  
            await appInit;
        },
        onRestart() {
            addLogConnection("Connection restarted", StatusEnum.success);
            callback.onRestart();
        },
        onStartFail(error) {
            addLog(error, StatusEnum.fail);
        },
        onclose() {
            addLogConnection("Connection closed", StatusEnum.fail);
            callback.onClose();
        },
        async onReceiveMessage(userName, postBody, roomId, createDate, identifier) {
            addLog(`Message received from ${userName} in room: ${roomId}`, StatusEnum.success);
            var post = {userName, postBody, roomId, createDate, identifier}
            // TODO : simplify - should just await onStart
            await appInit;
            callback.onReceive(post);
        },
        async onReceiveRoom(roomname, roomId) {
            addLog(`Room received with id: ${roomId}`, StatusEnum.success);
            callback.onReceiveRoom({name : roomname, id : roomId});
        },
        async onReceiveError(error) {
            addLog(`${error}`, StatusEnum.fail);
        }
    }

    connection.onclose(_internal.onclose);
    connection.on("ReceiveMessage", _internal.onReceiveMessage);
    connection.on("ErrorMessage", _internal.onReceiveError);
    connection.on("CreateRoomMessage", _internal.onReceiveRoom);


    return api;

};
