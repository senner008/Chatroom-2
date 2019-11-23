import {  actionRoomSelect, actionRoomSelectRender1, actionRoomSelectRender2 } from "../actions/action-room-select";
import { State } from "../State";
import { renderUsers } from "./render-create-room";
import { setUsers, createRoom } from "../actions/createRoom";
import { showModal } from "./render";



function roomClickListener () {
    $("#rooms-list").find('.rooms').on("click", (e) => {
        var id = Number(e.target.dataset.id);
        actionRoomSelect(id, actionRoomSelectRender1, actionRoomSelectRender2, true)
    });
}

// TODO : move logic to actions
// function sendClickListener() {
//     $("#sendButton").on("click", function (e) {
//         var message = (<HTMLInputElement> document.getElementById("messageInput")).value;
//         sendMessage(message);
//     });    
// }

// TODO : move logic to actions
function CreateRoomClickHandler() {
    $("#rooms-list .create").on("click", async () => {
        await setUsers()
        renderUsers(State.getUsers())
        showModal();
        usersClickHandler();
    });    
}

// TODO : move logic to render folder
function usersClickHandler() {
    $(".modal-body .user-list").off();
    $(".modal-body .user-list").on("click", handler)
    var publicLi = $(".modal-body .user-list").find("[data-user-nickname='public']");
    function handler (e) {
        if ( e.target.dataset.userNickname === "public" ) {
            $(".modal-body .user-list li").each((index,li) => li.classList.remove("user-select"))
            publicLi.addClass("user-select")
        } else {
            publicLi.removeClass("user-select")
            e.target.classList.toggle("user-select")
        }
    }
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
        createRoom(users, name);
        $('#room-create-modal').modal('hide');

    })
}


export function addListeners () {
    roomClickListener();
    CreateRoomClickHandler()
    usersClickHandler();
    modalSaveChangesClickHandler();
}
