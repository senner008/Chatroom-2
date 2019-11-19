"use strict";
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



var State = (function IIFE() {
    var state = {
        roomId : null,
        rooms : {}
    }

    var posts = {}
    return {
        async setState(prop, value) {
            if (!(prop in state)) {
                throw `invalid parameter of ${prop} to setState method`;
            }
            state[prop] = value;
            this.getPosts();
           
        },
        getState(prop) {
            if (!(prop in state)) {
                throw `invalid parameter of ${prop} to getState method`;
            }
            if (typeof state[prop] === "function") return state[prop]();
            return state[prop];
        },
        async getPosts() {
            // remove func
            posts[state.roomId] = await getPostsByRoomId(state.roomId);
            renderPostList(posts[state.roomId]);
            
        }
    }
})();


function roomNameById (id) {
    return State.getState("rooms").filter(room => room.id === Number(id))[0].name
}

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.onclose((error) => {
    console.assert(connection.state === signalR.HubConnectionState.Disconnected);
    $("#connection-state").text("Connectection state is off").css(color.red);
});

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, roomId) {
    console.log("message received");

    // var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    // var encodedMsg = user + " says " + msg;
    // var li = document.createElement("li");
    // li.textContent = encodedMsg;
    // document.getElementById("messagesList").appendChild(li);
    renderPost(user, message, roomId)
});
var color = {
    red : {color: "red"},
    green : {color: "green"}
}


async function StartConnection () {

    var tries = 1;
    (async function start() {
        if (connection.connectionState === "Connected" && tries !== 0) return;
        connection.start().then(function () {
            $("#connection-state").text("Connectection state is on").css(color.green)
            console.log("connection started...")
            document.getElementById("sendButton").disabled = false;
        }).catch(function (err) {
            $("#connection-state").text("Connectection state is off").css(color.red);
            
            console.error(err.toString());
            setTimeout(() => start(), tries--);
        });
    })();

}

StartConnection();

document.getElementById("sendButton").addEventListener("click", async function (event) {

    await StartConnection();
  
    // var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var roomId = Number(State.getState("roomId"));
    try {
        const response = await postData("home/sendmessage", {"message" : message, "roomId" : roomId} );
        if (!response.ok) {
            throw await response.text();
        }
        $("#connection-message").text("Message added to room" + roomNameById(State.getState("roomId"))).css(color.green);
    } catch (err) {
        $("#connection-message").text(err.toString()).css(color.red);
        return;
    }
    // .catch(function (err) {
    //         console.log("catching sendmessage errors...")
    //         return console.error(err.toString());
    //     });
    // connection.invoke("SendMessage", message, roomId).catch(function (err) {
    //     return console.error(err.toString());
    // });
    event.preventDefault();
});


async function postData(url = '', data = {}) {
    // Default options are marked with *
    return fetch(url, {
      method: 'POST', // *GET, POST, PUT, DELETE, etc.
      credentials: 'same-origin', // include, *same-origin, omit
      headers: {
        'Content-Type': 'application/json',
        "RequestVerificationToken": $('#hubsendpost input[name="__RequestVerificationToken"]').val()
        // 'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
  }

async function getPostsByRoomId(roomId) {
    try {
        const response = await postData(`Posts/${roomId}`);
        if (!response.ok) {
            throw await response.text();
        }
        return response.json();
    } catch (error) {
        console.log(error);
        return [];
    }
}

// create global error handler

async function getRooms() {
    return await fetch("Rooms").then(res => res.json());
}

function renderRooms(rooms, selector) {
    if (rooms.length === 0) return;
    var list = rooms.map(room => {
        return '<li data-id="' + room.id + '"' + 'class="list-group-item">' + room.name + '</li>';
    })
    $(selector).find('ul').append(list);
}

function addRoomListeners(selector) {
    $(selector).find('ul').on("click", function(e) {
        // console.log(e.target.dataset.id);
        State.setState("roomId", e.target.dataset.id);
        $(this).children().each((index, li) => li.classList.remove("room-selected"));
        e.target.classList.add("room-selected");
       
    });

}


function renderPostList(posts) {

    var list = "";
    var index = posts.length;
    while (index != 0) {
        let post = posts[index -1] 
        list += postElement(post.userName, post.postBody);
        index--;
    }

    $("#posts").html(list);
}

function postElement(user, msg) {
    return `<div class="post">
    <div class="user">
        ${user}
    </div>
        <p>${msg}</p>
    </div>`;
}



function renderPost(user, msg, roomId) {
    //fix Number conversion
    if (Number(State.getState("roomId")) !== roomId) return;
    // only render if in 
    $("#posts").append(postElement(user,msg));
}

(async function init () {
    State.setState("rooms",await getRooms());

    var roomsSelector = "#rooms-list";
    renderRooms(State.getState("rooms"), roomsSelector);
    addRoomListeners(roomsSelector);

})();


