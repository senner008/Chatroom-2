import { State } from "../State";
import { getAllUsers, sendCreateRoom } from "../ajaxMethods";
import { renderUsers, getNameRendered, getUserNicknamesSelected } from "../render/render-create-room";
import { showModal } from "../render/render";


export async function createRoom() {
    var users = await getAllUsers();
    State.setUsers(users);
    renderUsers(State.getUsers())
    showModal();
}


export function modalSaveChanges(render) {
    var users = getUserNicknamesSelected();
    var name = getNameRendered();
    sendCreateRoom(users, name);
    render();
}

export function modalSaveChangesRender() {
    $('#room-create-modal').modal('hide');
}

