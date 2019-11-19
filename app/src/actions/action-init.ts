import { renderRooms } from "../render/render-rooms";
import { renderView, showLoader } from "../render/render";
import { addListeners } from "../render/listeners";
import { getRooms } from "../ajaxMethods";
import { State } from "../State";
import { RoomsFactory } from "../Rooms";
import { Logger } from "../Logger";
import { StatusEnum } from "../Ajax";

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
    Logger.connectionState("Connectection state is on", StatusEnum.success);
    Logger.message("", StatusEnum.success);
}
