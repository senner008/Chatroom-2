import { State } from "../State";
import { renderPost } from "../render/render-posts";


export async function actionReceiveMessage(post, render) {
    console.log(post)
    State.addPost(post);
    render(post)
}

export async function actionReceiveMessageRender(post) {
    if (State.getActiveRoom() !== null && State.getActiveRoom().id == post.roomId) {  
        renderPost(post.userName, post.postBody);
    }
}
