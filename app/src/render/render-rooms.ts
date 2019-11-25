import { actionRoomSelect, actionRoomSelectRender1, actionRoomSelectRender2 } from "../actions/action-room-select";


const roomsList = $("#master-list");

export function renderRoomListSelect(id) {
    roomsList.find('li').each((index, li) => li.classList.remove("room-selected"));
    roomsList.find(`[data-id='${id}']`)[0].classList.add("room-selected");
}

export function pushState (id) {
    history.pushState(id, id, `/RoomInit/${id}`);
}

window.onpopstate = function(event) {
    actionRoomSelect(Number(event.state), actionRoomSelectRender1, actionRoomSelectRender2, false)
};

export function appendRoom (room) {
    var roomLi = `<li data-id='${room.id}' class="list-group-item">${room.name}</li>`;
    roomsList.find('.rooms').append(roomLi);
}


export function roomExists(id) {
    return roomsList.find(`[data-id='${id}']`).length > 0;
}

export async function renderRooms (rooms) {
    if (rooms.length === 0) return;
    var list = rooms.map(room => {
        return `<li data-id='${room.id}' class="list-group-item">${room.name}</li>`;
    })
    roomsList.find('.rooms').html(list);
}
