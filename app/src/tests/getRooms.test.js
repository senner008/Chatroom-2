import { getRooms } from "../ajaxMethods.ts";
import * as ajaxPostDependencies  from "../Ajax.ts";

test('getRooms() should return list of rooms', async () => {

    const rooms = [{name: 'Public', id: 1}];
    
    // replace ajaxPost with mock
    ajaxPostDependencies.ajaxPost = () => rooms;

    var res = ajaxPostDependencies.ajaxPost()
    console.log(res)
    
    var response = await getRooms();

    expect(response.length).toBe(1);
});

