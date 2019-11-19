import { State } from "../State";
import { getAllUsers, sendCreateRoom } from "../ajaxMethods";

export async function setUsers() {
    State.setUsers(await getAllUsers());
}

export async function createRoom(users, name) {
    sendCreateRoom(users, name);
}



