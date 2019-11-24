import { Logger } from "./GlobalLogger";


export async function ajaxPost<T>(url, data = {})  {
  return await ErrorHandler<T>(fetchAction(url, data));
}

function fetchAction(url, data) {
  return fetch(url, {
    method: 'POST', // *GET, POST, PUT, DELETE, etc.
    credentials: 'same-origin', // include, *same-origin, omit
    headers: {
      'Content-Type': 'application/json',
      "RequestVerificationToken": $('#hubsendpost input[name="__RequestVerificationToken"]').val() as string
      // 'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: JSON.stringify(data) // body data type must match "Content-Type" header
  });
}


export enum StatusEnum {
    success,
    fail
}

export async function ErrorHandler<T> (action) : Promise<T[]> {
    try {
        const response = await action;
        if (response.status === 302) {
          // console.log(location)
          window.location.replace(location.origin + "/Home/Error")
        }
        if (!response.ok) {
            throw await jsonOrText(response);
        }
        Logger.message(response.headers.get('Response-message'), StatusEnum.success);
        return await jsonOrText<T>(response);
    } catch (err) {
        console.log(err)
        // var error = err === 'object' ? err.error : err;
        Logger.message(err, StatusEnum.fail);
        return [] as any;
    }
}

async function jsonOrText<T> (response) : Promise<T[]> {
    const contentType = response.headers.get("content-type");
    if (contentType && contentType.indexOf("application/json") !== -1) {
      return await response.json();
    } else {
      const text = await response.text();
      return [text];
    }
}



