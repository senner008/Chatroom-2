import { renderView } from "../render/render";
import { StatusEnum } from "../Ajax";
import { Logger } from "../Logger";


export async function actionOncloseRender() {
    renderView(false);
    Logger.connectionState("Connectection state is off", StatusEnum.fail);
}