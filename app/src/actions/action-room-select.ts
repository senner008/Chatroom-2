
import { State } from "../State";
import { getPostsByRoomId } from "../ajaxMethods";
import { showLoader, renderPostInputField, renderView } from "../render/render";
import { renderRoomListSelect, roomExists, pushState } from "../render/render-rooms";
import { renderPostList, scrollToBottom } from "../render/render-posts";

export async function actionRoomSelect(id, render1, render2, shouldPushState) {
    if (!roomExists(id)) return 
    render1(id);
    var getPosts = await getPostsByRoomId(id)
    State.setActiveRoom(id, getPosts);
    render2();
    if (shouldPushState) pushState(id);
}

export async function actionRoomSelectRender1(id) {
    showLoader(true)
    renderRoomListSelect(id);
}
export async function actionRoomSelectRender2() {
    renderPostInputField(true);
    renderPostList(State.getActiveRoom().getPosts());
    renderView(true);
   
    showLoader(false)
    scrollToBottom();
}


