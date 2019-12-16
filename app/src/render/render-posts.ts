const postsSelectors = {
    posts : "#posts"
}

export function renderPostList(postslist) {
    const listToElems = postslist
        .map(post => postElement(post[0], post[1])).join("");

    $(postsSelectors.posts).html(listToElems);
}

export function renderPost(user, msg) {
    $(postsSelectors.posts)
        .append(postElement(user,msg));
}

export function postElement(user, msg) {

    const msgLines = msg.split("\n").map(line => {
        return `<p class="post-message">${line}</p>`
    }).join("");

    return `<div class="post">
        <div class="user">
            ${user}
        </div>
        <div class="post-message-container">
            ${msgLines}
        </div>
    </div>`;
}

export function scrollToBottom() {
    $(postsSelectors.posts)[0].scrollTop = $(postsSelectors.posts)[0].scrollHeight;
}