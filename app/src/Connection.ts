import { RoomsFactory } from "./Rooms";
import { State } from "./State";
import { getRooms } from "./ajaxMethods";


interface IHubConnection {
    onclose : (callback) => void;
    on : (name, callback) => void
    state : number
    start : () => void
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
        onStartRender(cb) {
            callback['onStart'] = cb; 
        },
        onCloseRender(cb) {
            callback['onClose'] = cb; 
        },
        onReceiveRender(cb) {
            callback['onReceive'] = cb; 
        },
        onReceiveRoomRender(cb) {
            callback['onReceiveRoom'] = cb; 
        },
        onRestartRender(cb) {
            callback['onRestart'] = cb; 
        },
        async start() {
            await StartConnection(_internal.onStart);
        },
        async restart() {
            await StartConnection(_internal.onRestart);
         
        }
    }

    const _internal = {
        async onStart() {
            appInit = new Promise(async (res, rej) => {
                addLog("Connection started");
                var rooms = await getRooms();
                State.setRooms(RoomsFactory(rooms));
                await callback.onStart(rooms);
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
        async onReceive(userName, postBody, roomId, createDate) {
            addLog(`Message reveived from ${userName} in room: ${roomId}`);
            await appInit;
            var post = {userName, postBody, roomId, createDate}
            State.addPost(post);
            callback.onReceive(post);
        },
        async onReceiveRoom(message) {
            console.log("room message recived :")
            console.log(message)
            var rooms = await getRooms();
            State.setRooms(RoomsFactory(rooms));
            callback.onReceiveRoom(rooms);
        }
    }

    connection.onclose(_internal.onclose);
    connection.on("ReceiveMessage", _internal.onReceive);
    connection.on("CreateRoomMessage", _internal.onReceiveRoom);

    return api;

};
