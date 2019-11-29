
    
export const RoomRender = (() =>  {
    const _roomsList = "#master-list .rooms";
    const _findById =  id => $(_roomsList).find(`[data-id='${id}']`);
    const _createLi =  room => `<li data-id='${room.id}' class="list-group-item">${room.name}</li>`;

    const self  = {
            append : room => $(_roomsList).append(_createLi(room)),
            exists : id => _findById(id).length > 0,
            renderList : (rooms) => {
                if (rooms.length === 0) return;
                $(_roomsList).html(rooms.map(_createLi));
            },
            renderSelect : id => {
                $(_roomsList).find('li').removeClass("room-selected");
                _findById(id).addClass("room-selected");
            }
        }
    return self;
})();


