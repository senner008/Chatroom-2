

import { StatusEnum } from "./Ajax";
import { headerRender } from "./render/render";

const color = {
    red : "alert-danger",
    green : "alert-success"
}

const Logger  = (function() {
    return Object.freeze({
        message(msg  : string, status : StatusEnum) {
            headerRender.message(msg, status == StatusEnum.success ? color.green : color.red);
        },
        connectionState(msg  : string, status : StatusEnum) {
            headerRender.state(msg, status == StatusEnum.success ? color.green : color.red);
        }
    });
}());

export { Logger}