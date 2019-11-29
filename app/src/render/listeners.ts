import {  actionRoomSelect, actionRoomSelectRender1, actionRoomSelectRender2 } from "../actions/action-room-select";
import { createRoom } from "../actions/action-create-room";



function roomClickListener () {
    $("#master-list").find('.rooms').on("click", (e) => {
        var id = Number(e.target.dataset.id);
        actionRoomSelect(id, actionRoomSelectRender1, actionRoomSelectRender2, true)
    });
}

function createRoomClickHandler() {
    $("#master-list .create").on("click", async () => {
        createRoom();
    });    
}


export function modalSaveChangesClickHandler() {
    $("#modal-container").on('click', ".save-changes", () => {
        $('#modal').modal('hide');
    });
}

window.onpopstate = function(event) {
    actionRoomSelect(Number(event.state), actionRoomSelectRender1, actionRoomSelectRender2, false)
};

export function addListeners () {
    roomClickListener();
    createRoomClickHandler();
    modalSaveChangesClickHandler();
}

