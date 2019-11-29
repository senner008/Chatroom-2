

// TODO : refactor
const headerRender = (function IIFE ()  {

    function switchClass(elem, msg, color) {
        elem.find(".alert").removeClass("alert-success").removeClass("alert-danger")
        elem.find(".alert").text(msg).addClass(msg ? color : "");
    }
    
    return {
        message (msg, color) {
            switchClass($("#connection-message"), msg, color);
        },
        state (msg, color) {
            switchClass($("#connection-state"), msg, color);
        }
    }
})();

export const renderModal = (modal) => {
    $("#modal-container").html(modal);
    $('#modal').modal('show');
    return $('#modal');
}


export function renderPostInputField (isOn : boolean) {
    $("#posts-container #post-input")[isOn ? 'show' : 'hide']();
}

export function showLoader(isOn : boolean) {
    $("#posts-container .loader")[isOn ? 'show' : 'hide']();
    $("#posts-container #posts")[isOn ? 'hide' : 'show']();
}

// TODO : get all elems with class state-conditional
export function renderView(isTrue : boolean) : void {
    $("#reconnect-button")[isTrue ? 'hide' : 'show'](); 
    $(".connection-conditional")[isTrue ? 'show' : 'hide'](); 
}

export function pushState (id) {
    history.pushState(id, id, `/RoomInit/${id}`);
}


export { headerRender }
