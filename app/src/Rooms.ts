
import { IRoom, IPost } from "./ajaxMethods";
import { postElement } from "./render/render-posts";

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
            console.log(post.identifier in posts)
            posts[post.identifier] = {
                postBody : post.postBody,
                postUser : post.userName,
                roomId : id,
                createDate : post.createDate,
                createDateNumber :  new Date(post.createDate).getTime()
            }
            console.log(posts)
        },
        getSortedPostsList() {
            // TODO : show create date
            var list = "";
            Object.keys(posts)
            .sort((a, b) => (posts[a].createDateNumber > posts[b].createDateNumber) ? 1 : -1)
            .forEach(function(v, i) {
                list += postElement(posts[v].postUser, posts[v].postBody);
            });
            return list;
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


  