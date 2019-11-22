import { State } from "../State";
import { RoomsFactory } from "../Rooms";
import { renderRooms } from "../render/render-rooms";
import { getRooms } from "../ajaxMethods";


export async function actionReceiveRoom(room, render) {
    State.setRooms(RoomsFactory([room]));
    // TODO : should not call getRooms when receiving a room
    var rooms = await getRooms();
    render(rooms)
}

export function actionReceiveRoomRender(rooms) {
    renderRooms(rooms)
}