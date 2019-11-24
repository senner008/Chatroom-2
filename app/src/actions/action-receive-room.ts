import { State } from "../State";
import { RoomsFactory } from "../Rooms";
import { renderRooms, appendRoom } from "../render/render-rooms";
import { getRooms } from "../ajaxMethods";


export async function actionReceiveRoom(room, render) {
    State.setRooms(RoomsFactory([room]));
    // TODO : should not call getRooms when receiving a room
    render(room)
}

export function actionReceiveRoomRender(room) {
    appendRoom(room)
}