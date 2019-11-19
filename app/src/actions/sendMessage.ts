import { ajaxPost } from "../Ajax";
import { State } from "../State";

export async function sendMessage(message : string) : Promise<void> {
    const roomId = State.getActiveRoom().id;
    await ajaxPost<string>("hub/sendmessage", {"message" : message, "roomId" : roomId}, "");
}

