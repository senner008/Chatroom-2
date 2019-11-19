import {  roomShow } from "../actions/roomSelect";
import { sendMessage } from "../actions/sendMessage";

import { renderRoomListSelect } from "./render-rooms";
import { State } from "../State";
import { renderPostInputField, showLoader, showModal } from "./render";
import { renderPostList } from "./render-posts";
import { renderUsers } from "./render-create-room";
import { setUsers, createRoom } from "../actions/createRoom";


function roomClickListener () {
    $("#rooms-list").find('.rooms').on("click", async function (e) {
        showLoader(true)
        var id = Number(e.target.dataset.id)
        renderRoomListSelect(id);
        await roomShow(id)
        renderPostInputField(true);
        renderPostList(State.getActiveRoom().posts);
        showLoader(false)
    });
}

function sendClickListener() {
    $("#sendButton").on("click", function (e) {
        var message = (<HTMLInputElement> document.getElementById("messageInput")).value;
        sendMessage(message);
    });    
}

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
            console.log("dsds")
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
    sendClickListener();
    usersClickHandler();
    modalSaveChangesClickHandler();
}
