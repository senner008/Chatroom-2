

const headerRender = (function IIFE ()  {

    function removeClasses(elem) {
        elem.find(".alert").removeClass("alert-success").removeClass("alert-danger")
    }
    var cnmessage = $("#connection-message");
    var stateMessage = $("#connection-state");

    return {
        message (msg, color) {
            removeClasses(cnmessage);
            cnmessage.find(".alert").text(msg).addClass(msg ? color : "");
        },
        state (msg, color) {
            removeClasses(stateMessage);
            stateMessage.find(".alert").text(msg).addClass(msg ? color : "");
        }
    }
})();


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
    $("#posts-container main")[isTrue ? 'show' : 'hide'](); 
    $("#master-list .create")[isTrue ? 'show' : 'hide'](); 
    $("#master-list .rooms")[isTrue ? 'show' : 'hide'](); 
    $("#connection-message")[isTrue ? 'show' : 'hide'](); 
    $("#reconnect-button")[isTrue ? 'hide' : 'show'](); 
}

export { headerRender }
