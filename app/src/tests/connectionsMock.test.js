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

var onStart_Callback = jest.fn();
var onLogConnection_Callback = jest.fn();


test('connection should fire onStartRender callback on start', async () => {

    var connection = Connection(connectionMock);
    connection.onStart(onStart_Callback);
    connection.onLogConnection(onLogConnection_Callback);
    await connection.start();
    // Assert
    expect(onStart_Callback).toHaveBeenCalledTimes(1);

});


test('State active room should be null on start', async () => {
   
    var connection = Connection(connectionMock);
    connection.onStart(onStart_Callback);
    connection.onLogConnection(onLogConnection_Callback);
    await connection.start();

    // Assert
    expect(State.getActiveRoom()).toBeNull();

});

test('Connection start should fire onLogConnection callback', async () => {
   
    var connection = Connection(connectionMock);
    connection.onStart(onStart_Callback);
    connection.onLogConnection(onLogConnection_Callback);
    await connection.start();

    // Assert
    expect(onLogConnection_Callback).toHaveBeenCalledTimes(1);

});


