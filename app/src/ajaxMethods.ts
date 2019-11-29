import { ajaxPost } from "./Ajax";

export interface IPost {
    postBody: string;
    userName: string;
    createDate: number;
    roomId: number,
    identifier : string;
}

export interface IUser {
    nickName: string;
}

export interface IRoom {
    name: string,
    id: number
}


export async function getPostsByRoomId(roomId) {
    return await ajaxPost<IPost>(`/Posts/${roomId}`, "");
}

export async function getAllUsers() {
    return await ajaxPost<IUser>(`/Users`, "");
}

export async function getRooms() {
    return await ajaxPost<IRoom>("/Rooms", "");
}

export async function sendCreateRoom(users, name) {
    console.log(users)
    return await ajaxPost<IRoom>("/rooms/create", { roomname : name , UserList : users });
}



