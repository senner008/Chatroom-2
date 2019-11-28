import {  actionRoomSelect, actionRoomSelectRender1, actionRoomSelectRender2 } from "../actions/action-room-select";
import { State } from "../State";
import { renderUsers, userlistSelectRender } from "./render-create-room";
import { createRoom, modalSaveChanges, modalSaveChangesRender } from "../actions/action-create-room";
import { showModal } from "./render";
import { sendCreateRoom } from "../ajaxMethods";


function roomClickListener () {
    $("#master-list").find('.rooms').on("click", (e) => {
        var id = Number(e.target.dataset.id);
        actionRoomSelect(id, actionRoomSelectRender1, actionRoomSelectRender2, true)
    });
}


function CreateRoomClickHandler() {
    $("#master-list .create").on("click", async () => {
        createRoom();
    });    
}


function usersClickHandler() {
    $(".modal-body .user-list").on("click", userlistSelectRender)
}


function modalSaveChangesClickHandler() {
    $(".modal-footer .save-changes").on('click', () => {
        modalSaveChanges(modalSaveChangesRender);
    });
}


export function addListeners () {
    roomClickListener();
    CreateRoomClickHandler()
    usersClickHandler();
    modalSaveChangesClickHandler();
}
