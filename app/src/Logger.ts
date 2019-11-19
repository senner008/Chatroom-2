
import { renderConnectionMessage, renderConnectionState } from "./render/render";
import { StatusEnum } from "./Ajax";

const color = {
    red : {color: "red"},
    green : {color: "green"}
}

// const messages = {
//     messageAdded() { return "Message added to room" + State.getRoomNameById(State.getRoomId()) },
//     roomsRetrieved() { return "Rooms retrieved"},
//     retrievedPosts() { return "successfully retireved posts for " + State.getRoomNameById(State.getRoomId())}
// }

// TODO : Log to session storage. Should display messages comming from server
const Logger  = (function() {

    return Object.freeze({
        message(msg  : string, status : StatusEnum) {
            if (status == StatusEnum.success)
            {
                renderConnectionMessage(msg,color.green);
            }
            else {
                renderConnectionMessage(msg,color.red);
            }
        },
        connectionState(msg  : string, status : StatusEnum) {
            if (status == StatusEnum.success)
            {
               renderConnectionState(msg,color.green);
            }
            else {
                renderConnectionState(msg,color.red);
            }
        }
    });
}());

export { Logger}