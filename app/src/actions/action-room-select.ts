
import { State } from "../State";
import { getPostsByRoomId } from "../ajaxMethods";
import { showLoader, renderPostInputField } from "../render/render";
import { renderRoomListSelect } from "../render/render-rooms";
import { renderPostList } from "../render/render-posts";

export async function actionRoomSelect(e, render1, render2) {
    
    var id = Number(e.target.dataset.id)
    render1(id);
    var getPosts = await getPostsByRoomId(id)
    await State.setActiveRoom(id, getPosts);
    render2();
}

export async function actionRoomSelectRender1(id) {
    showLoader(true)
    renderRoomListSelect(id);
}
export async function actionRoomSelectRender2() {
    renderPostInputField(true);
    renderPostList(State.getActiveRoom().posts);
    showLoader(false)
}


