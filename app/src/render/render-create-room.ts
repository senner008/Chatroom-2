export function renderUsers(users) {
    var publicLi = [`<li data-user-nickname="public" class='list-group-item'>Public</li>`];
    var usersLis = users.map(user => `<li data-user-nickname="${user.nickName}" class="list-group-item">${user.nickName}</li>`);
    $(".modal-body .user-list").html(publicLi.concat(usersLis).join(""));
}
