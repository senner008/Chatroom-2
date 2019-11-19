import { renderView } from "../render/render";
import { Logger } from "../Logger";
import { StatusEnum } from "../Ajax";

export async function actionOnRestartRender() {
    renderView(true);
    Logger.connectionState("Connectection state is on", StatusEnum.success);
    Logger.message("", StatusEnum.success);
}