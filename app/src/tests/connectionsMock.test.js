import { Connection } from "../Connection.ts";
import {State} from "../State.ts";
import * as ajaxPostDependencies  from "../Ajax.ts";

var connectionMock = {
    onclose(cb) {
    },
    start() {
        return true;
    },
    on(func, cb) {

    },
    state : 0
}

// replace ajaxPost with mock
ajaxPostDependencies.ajaxPost = () => [{name: 'Public', id: 1}];
// replace asynCallback with mock
var asynCallback = jest.fn();


test('connection should fire onStartRender callback on start', async () => {
 
    var connection = Connection(connectionMock);
    connection.onStartRender(asynCallback);
    await connection.start();
    // Assert
    expect(asynCallback).toHaveBeenCalledTimes(1);

});


test('State active room should be null on start', async () => {
   
    var connection = Connection(connectionMock);
    connection.onStartRender(asynCallback);
    await connection.start();

    // Assert
    expect(State.getActiveRoom()).toBeNull();

});


test('connection should pass rooms to onStartRender callback on start', async () => {
    
    var connection = Connection(connectionMock);
    connection.onStartRender(async (rooms) => {
        // Assert
        expect(rooms[0].name).toBe("Public");
    });
    await connection.start();

});


