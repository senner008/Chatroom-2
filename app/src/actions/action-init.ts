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

    var routeElem = $("#chatroom-route-config")[0];
    if (routeElem) {
        actionRoomSelect(routeElem.dataset.initRoom, actionRoomSelectRender1, actionRoomSelectRender2, false)
    }
}
