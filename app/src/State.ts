import { IRoomClass } from "./Rooms";

interface IState {
    rooms : any;
    activeRoom : number;
    users : any
}

const State = (function IIFE() {
    var state : IState = {
        rooms : {},
        activeRoom : null,
        users : null
    }

    return Object.freeze({
        async setActiveRoom(id, posts) {
            if(id === null) state.activeRoom = null;
            state.activeRoom = id;
            await state.rooms[id].onActive(posts);
        },
        getActiveRoom() {
            return state.activeRoom ? state.rooms[state.activeRoom] : null;
        },
        addPost(post) : void{
            state.rooms[post.roomId].addPost(post);
        },
        setRooms(roomObjects) : void {
            for (let room in roomObjects) {
                state.rooms[room] = roomObjects[room]
            }
        },
        setUsers(users) {
            state.users = users;
        },
        getUsers() {
            return state.users;
        }
    });
})();


export {State};