const postsSelectors = {
    posts : "#posts"
}

export function renderPostList(postslist) {
    $(postsSelectors.posts).html(postslist);
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