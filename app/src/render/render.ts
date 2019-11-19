
export function renderConnectionMessage (msg, color) {
    $("#connection-message").text(msg).css(color);
}

export function renderConnectionState (msg, color) {
    $("#connection-state").text(msg).css(color);
}

export function showModal() {
    $('#room-create-modal').modal('show');
}

export function renderPostInputField (isOn : boolean) {
    $("#posts-container #post-input")[isOn ? 'show' : 'hide']();
}

export function showLoader(isOn : boolean) {
    $("#posts-container .loader")[isOn ? 'show' : 'hide']();
    $("#posts-container #posts")[isOn ? 'hide' : 'show']();
}

export function renderView(isTrue : boolean) : void {
    $("#posts-container")[isTrue ? 'show' : 'hide'](); 
    $("#connection-message")[isTrue ? 'show' : 'hide'](); 
    $("#reconnect-button")[isTrue ? 'hide' : 'show'](); 
}
