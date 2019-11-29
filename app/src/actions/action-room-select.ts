
import { State } from "../State";
import { getPostsByRoomId } from "../ajaxMethods";
import { showLoader, renderPostInputField, renderView, pushState } from "../render/render";
import { RoomRender } from "../render/render-rooms";
import { renderPostList, scrollToBottom } from "../render/render-posts";

export async function actionRoomSelect(id, render1, render2, shouldPushState) {
    if (!RoomRender.exists(id)) return 
    render1(id);
    var getPosts = await getPostsByRoomId(id)
    State.setActiveRoom(id, getPosts);
    render2();
    if (shouldPushState) pushState(id);
}

export async function actionRoomSelectRender1(id) {
    showLoader(true)
    RoomRender.renderSelect(id);
}
export async function actionRoomSelectRender2() {
    renderPostInputField(true);
    renderPostList(State.getActiveRoom().getSortedPostsList());
    renderView(true);
    showLoader(false)
    scrollToBottom();
}


