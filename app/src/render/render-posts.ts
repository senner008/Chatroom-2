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

    var decoded = $('<textarea/>').html(msg).text();

    function htmlEnc(s) {
        return s.replace(/&/g, '&amp;')
          .replace(/</g, '&lt;')
          .replace(/>/g, '&gt;')
          .replace(/'/g, '&#39;')
          .replace(/"/g, '&#34;');
      }

    const msgLines = decoded.split("\n").map(line => {
        return `<p class="post-message">${htmlEnc(line)}</p>`
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