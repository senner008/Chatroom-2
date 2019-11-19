
import { IRoom, IPost } from "./ajaxMethods";

export interface IRoomClass {
    posts: any;
    id : number;
    onActive : (posts) => void;
    addPost : (post : IPost) => void;

}

function RoomClass (name : string, id : number) : IRoomClass {

    var posts = {};
    // TODO : add room logging
    return {
        posts: posts,
        id: id,
        async onActive(posts) {
            posts.forEach(post => this.addPost(post));
        },
        addPost(post : IPost) {
            posts[Number(post.createDate)] = [post.postBody, post.userName, id];
        }
    }
}

export function RoomsFactory(rooms : IRoom[]) {
    var roomsMap = {}
    rooms.forEach(room => {
        roomsMap[room.id]  = RoomClass(room.name, room.id) 
    })
    return roomsMap
}


  