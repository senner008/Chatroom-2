const postsSelectors = {
    posts : "#posts"
}

export function renderPostList(posts) {
    var list = "";

    if (posts) {
        Object.keys(posts)
        .sort((a: any, b : any) =>  a-b )
        .forEach(function(v, i) {
            list += postElement(posts[v][1], posts[v][0]);
        });
    }
    $(postsSelectors.posts).html(list);
}

export function renderPost(user, msg) {
    $(postsSelectors.posts).append(postElement(user,msg));
}

export function postElement(user, msg) {
    return `<div class="post">
    <div class="user">
        ${user}
    </div>
        <p class="post-message">${msg}</p>
    </div>`;
}