
const modal = {
    "userlist" : ".modal-body .user-list",
    "userselect" : "user-select"
};

const modalUserList = ".modal-body .user-list";

export function renderUsers(users) {
    var publicLi = userLiElement("public");
    var userLiElements = users.map(user => userLiElement(user.nickName));
    $(modalUserList).html(publicLi.concat(userLiElements).join(""));
}

const userLiElement = (nickName) => [`<li data-user-nickname=${nickName} class='list-group-item'>${nickName}</li>`];

export function getUserNicknamesSelected() {
    return  getUserLis()
        .filter(li => li.classList.contains(modal.userselect))
        .map((li : HTMLElement) => li.dataset.userNickname)
}

const getUserLis = () => [...document.querySelector(modalUserList).children];


export function getNameRendered() {
    return $(".modal-body").find(".room-name").val();
}

export function userlistSelectRender(e) {
   
    const removeClasses = () => getUserLis().forEach(li => li.classList.remove(modal.userselect));
    const publicLi = getUserLis().filter((li : HTMLElement) => li.dataset.userNickname === "public")[0];

    if ( $(e.target).is(publicLi) ) {
        removeClasses();
        publicLi.classList.add(modal.userselect)
    } else {
        publicLi.classList.remove(modal.userselect)
        e.target.classList.toggle(modal.userselect)
    }
}