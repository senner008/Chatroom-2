import { State } from "../State";
import { RoomsFactory } from "../Rooms";
import { RoomRender } from "../render/render-rooms";


export async function actionReceiveRoom(room, render) {
    State.setRooms(RoomsFactory([room]));
    // TODO : should not call getRooms when receiving a room
    render(room)
}

export function actionReceiveRoomRender(room) {
    RoomRender.append(room)
}