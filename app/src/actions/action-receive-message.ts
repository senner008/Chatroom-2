import { State } from "../State";
import { renderPost, scrollToBottom } from "../render/render-posts";


export async function actionReceiveMessage(post, render) {
    State.addPost(post);
    render(post)
}

export async function actionReceiveMessageRender(post) {
    if (State.getActiveRoom() !== null && State.getActiveRoom().id == post.roomId) {  
        renderPost(post.userName, post.postBody);
    }
    scrollToBottom();
}
