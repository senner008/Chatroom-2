import { State } from "../State";
import { renderPost } from "../render/render-posts";
import { Logger } from "../Logger";
import { StatusEnum } from "../Ajax";


export async function actionReceiveError(error, render) {
    console.log(error)
    render(error)
}

export async function actionReceiveErrorRender(error) {
    Logger.message(error, StatusEnum.fail);
}
