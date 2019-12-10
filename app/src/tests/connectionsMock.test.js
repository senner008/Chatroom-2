import { Connection } from "../Connection.ts";
import {State} from "../State.ts";
import * as ajaxPostDependencies  from "../Ajax.ts";
import {actionInit}  from "../actions/action-init.ts";

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

test('actionInit should update state', async () => {

    // replace ajaxPost with mock
    ajaxPostDependencies.ajaxPost = () => [{name: 'Public', id: 1}];

   
    const triggerInitRoom =  jest.fn();
    const actionInitRender =  jest.fn();

    await actionInit(actionInitRender, triggerInitRoom);  
    var state = State.getState();

    // Assert
    expect(actionInitRender).toHaveBeenCalledTimes(1);
    expect(triggerInitRoom).toHaveBeenCalledTimes(1);

    expect(Object.keys(state.rooms)).toStrictEqual(["1"]);


});
