
export function renderRoomListSelect(id) {
    $("#rooms-list").find('li').each((index, li) => li.classList.remove("room-selected"));
    $("#rooms-list").find("[data-id='" + id + "']")[0].classList.add("room-selected");
}

export async function renderRooms (rooms) {
    if (rooms.length === 0) return;
    var list = rooms.map(room => {
        return '<li data-id="' + room.id + '"' + 'class="list-group-item">' + room.name + '</li>';
    })
    $("#rooms-list").find('.rooms').html(list);
}
