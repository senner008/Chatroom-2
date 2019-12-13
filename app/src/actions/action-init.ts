import { RoomRender } from "../render/render-rooms";
import { renderView, showLoader } from "../render/render";
import { addListeners } from "../render/listeners";
import { State } from "../State";
import { RoomsFactory } from "../Rooms";
import { getRooms } from "../ajaxMethods";

export async function actionInit(render, trigger) {
    var rooms = await getRooms();
    State.setRooms(RoomsFactory(rooms));
    render(rooms);
    trigger();
}

export function actionInitRender(rooms) {
    RoomRender.renderList(rooms);
    renderView(true);
    addListeners();
    showLoader(false);
}

