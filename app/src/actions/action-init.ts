import { renderRooms } from "../render/render-rooms";
import { renderView, showLoader } from "../render/render";
import { addListeners } from "../render/listeners";
import { getRooms } from "../ajaxMethods";
import { State } from "../State";
import { RoomsFactory } from "../Rooms";


export async function actionInit(render) {
    var rooms = await getRooms();
    State.setRooms(RoomsFactory(rooms));
    render(rooms);
}

export function actionInitRender(rooms) {
    renderRooms(rooms);
    renderView(true);
    addListeners();
    showLoader(false);
}
