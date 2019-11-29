import { State } from "../State";
import { getAllUsers, sendCreateRoom } from "../ajaxMethods";

import { CreateRoomModalState, CreateRoomModal } from "../render/render-create-room";

export async function createRoom() {
    var users = await getAllUsers();
    State.setUsers(users);
    var modal = CreateRoomModalState(CreateRoomModal(users));
    modal.onSaveShanges(function () {
        var users = modal.getUserNicknamesSelected();
        var name = modal.getNameRendered();
        sendCreateRoom(users, name);
    });
}


