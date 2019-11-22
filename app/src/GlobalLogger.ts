
import { renderConnectionMessage, renderConnectionState } from "./render/render";
import { StatusEnum } from "./Ajax";

const color = {
    red : {color: "red"},
    green : {color: "green"}
}

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