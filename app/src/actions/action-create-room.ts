import { State } from "../State";
import { getAllUsers, sendCreateRoom } from "../ajaxMethods";
import { renderUsers } from "../render/render-create-room";
import { showModal } from "../render/render";


export async function createRoom() {
    var users = await getAllUsers();
    State.setUsers(users);
    renderUsers(State.getUsers())
    showModal();
}


