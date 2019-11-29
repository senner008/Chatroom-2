
    
export const RoomRender = (() =>  {
    const roomsList = "#master-list .rooms";
    const findById =  id => $(roomsList).find(`[data-id='${id}']`);
    const createLi =  room => `<li data-id='${room.id}' class="list-group-item">${room.name}</li>`;

    const self  = {
            append : room => $(roomsList).append(createLi(room)),
            exists : id => findById(id).length > 0,
            renderList : (rooms) => {
                if (rooms.length === 0) return;
                $(roomsList).html(rooms.map(createLi));
            },
            renderSelect : id => {
                $(roomsList).find('li').removeClass("room-selected");
                findById(id).addClass("room-selected");
            }
        }
    return self;
})();


