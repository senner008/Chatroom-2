import {  actionRoomSelect, actionRoomSelectRender1, actionRoomSelectRender2 } from "../actions/action-room-select";
import { State } from "../State";
import { renderUsers, userlistSelectRender } from "./render-create-room";
import { createRoom } from "../actions/action-create-room";
import { showModal } from "./render";
import { sendCreateRoom } from "../ajaxMethods";


function roomClickListener () {
    $("#master-list").find('.rooms').on("click", (e) => {
        var id = Number(e.target.dataset.id);
        actionRoomSelect(id, actionRoomSelectRender1, actionRoomSelectRender2, true)
    });
}


// TODO : move logic to actions
function CreateRoomClickHandler() {
    $("#master-list .create").on("click", async () => {
        createRoom();
    });    
}

// TODO : move logic to render folder
function usersClickHandler() {
    $(".modal-body .user-list").on("click", userlistSelectRender)
}

// TODO : move logic to render folder

function modalSaveChangesClickHandler() {
    $(".modal-footer .save-changes").on('click', function () {
        var users = [];
        $(".modal-body .user-list li").each((index,li) => {
            if ($(li).hasClass("user-select")) {
                users.push(li.dataset.userNickname)
            }
        });
        var name = $(".modal-body").find(".room-name").val();
        sendCreateRoom(users, name);
        $('#room-create-modal').modal('hide');

    })
}


export function addListeners () {
    roomClickListener();
    CreateRoomClickHandler()
    usersClickHandler();
    modalSaveChangesClickHandler();
}
