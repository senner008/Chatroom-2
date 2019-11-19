import { State } from "../State";
import { renderPost } from "../render/render-posts";
import { Logger } from "../Logger";
import { StatusEnum } from "../Ajax";


export async function actionReceiveMessage(post, render) {
    State.addPost(post);
    render(post)
}

export async function actionReceiveMessageRender(post) {
    if (State.getActiveRoom() !== null && State.getActiveRoom().id == post.roomId) {  
        renderPost(post.userName, post.postBody);
    }
    Logger.message(`post message received in room: ${post.roomId}`, StatusEnum.success);
}
