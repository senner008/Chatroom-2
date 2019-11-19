import { State } from "../State";
import { Logger } from "../Logger";
import { StatusEnum } from "../Ajax";
import { RoomsFactory } from "../Rooms";
import { renderRooms } from "../render/render-rooms";
import { getRooms } from "../ajaxMethods";


export async function actionReceiveRoom(room, render) {
    State.setRooms(RoomsFactory([room]));
    Logger.message(`new room added`, StatusEnum.success);
    var rooms = await getRooms();
    render(rooms)
}

export function actionReceiveRoomRender(rooms) {
    renderRooms(rooms)
}