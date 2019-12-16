
import { IRoom, IPost } from "./ajaxMethods";

export interface IRoomClass {
    posts: any;
    id : number;
    onActive : (posts) => void;
    addPost : (post : IPost) => void;
    getSortedPostsList : () => any;
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
            posts[post.identifier] = {
                postBody : post.postBody,
                postUser : post.userName,
                roomId : id,
                createDate : post.createDate,
                createDateNumber :  new Date(post.createDate).getTime()
            }
        },
        getSortedPostsList() {
            // TODO : show create date
            return Object.keys(posts)
            .sort((a, b) => (posts[a].createDateNumber > posts[b].createDateNumber) ? 1 : -1)
            .map((post, i) => [posts[post].postUser, posts[post].postBody]);
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


  