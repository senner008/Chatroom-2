const modalUserList = ".modal-body .user-list";

export function renderUsers(users) {
    var publicLi = [`<li data-user-nickname="public" class='list-group-item'>Public</li>`];
    var usersLis = users.map(user => `<li data-user-nickname="${user.nickName}" class="list-group-item">${user.nickName}</li>`);
    $(modalUserList).html(publicLi.concat(usersLis).join(""));
}


export function getUsersRendered() {
    var users = [];
    $(".modal-body .user-list li").each((index,li) => {
        if ($(li).hasClass("user-select")) {
            users.push(li.dataset.userNickname)
        }
    });
    return users;
}

export function getNameRendered() {
    return $(".modal-body").find(".room-name").val();
}


export function userlistSelectRender(e) {
 
    const removeClasses = () => $(modalUserList).find("li").each((index,li) => li.classList.remove("user-select")); 

    var publicLi = $(modalUserList).find("[data-user-nickname='public']");

    if ( $(e.target).is(publicLi) ) {
        removeClasses();
        publicLi.addClass("user-select")
    } else {
        publicLi.removeClass("user-select")
        e.target.classList.toggle("user-select")
    }

}