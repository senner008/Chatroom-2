import { renderView } from "../render/render";


export async function actionOnRestartRender() {
    renderView(true);
}