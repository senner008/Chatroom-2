
import { State } from "../State";
import { getPostsByRoomId } from "../ajaxMethods";

export async function roomShow(id) {
    var getPosts = await getPostsByRoomId(id)
    await State.setActiveRoom(id, getPosts);
}


