import { renderRooms } from "../render/render-rooms";
import { renderView, showLoader } from "../render/render";
import { addListeners } from "../render/listeners";
import { getRooms } from "../ajaxMethods";
import { State } from "../State";
import { RoomsFactory } from "../Rooms";
import { actionRoomSelect, actionRoomSelectRender1, actionRoomSelectRender2 } from "./action-room-select";


export async function actionInit(render) {
    var rooms = await getRooms();
    State.setRooms(RoomsFactory(rooms));
    render(rooms);
    triggerInitRoom();
}

export function actionInitRender(rooms) {
    renderRooms(rooms);
    renderView(true);
    addListeners();
    showLoader(false);
}

function triggerInitRoom () {

    var header = $("#chatroom-header")[0];
    var initRoom = header.dataset.initRoom
    if (initRoom) {
        actionRoomSelect(initRoom, actionRoomSelectRender1, actionRoomSelectRender2, false)
    }
}
