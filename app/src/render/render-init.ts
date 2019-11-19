import { renderRooms } from "./render-rooms";
import { renderView, showLoader } from "./render";
import { addListeners } from "./listeners";


export function renderInit(rooms) {
    renderRooms(rooms);
    renderView(true);
    addListeners();
    showLoader(false);
}
