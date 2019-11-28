import { State } from "../State";
import { getAllUsers, sendCreateRoom } from "../ajaxMethods";
import { renderUsers, getUsersRendered, getNameRendered } from "../render/render-create-room";
import { showModal } from "../render/render";


export async function createRoom() {
    var users = await getAllUsers();
    State.setUsers(users);
    renderUsers(State.getUsers())
    showModal();
}


export function modalSaveChanges(render) {
    var users = getUsersRendered();
    var name = getNameRendered();
    sendCreateRoom(users, name);
    render();
}

export function modalSaveChangesRender() {
    $('#room-create-modal').modal('hide');
}

