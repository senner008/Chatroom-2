import { Modal } from "./render-modal";
import { renderModal } from "./render";

const modalSelectors = {
    "userlist" : ".modal-body .user-list",
    "userselect" : "user-select"
};
const modalUserList = ".modal-body .user-list";

export const CreateRoomModal = (userlist) => {

    const modalconfig = {
        content : (userlist) =>  `<div class="input-group">
                    <div class="input-group-prepend">
                        <span class="input-group-text" id="">Room name</span>
                    </div>
                        <input type="text" class="form-control room-name">
                </div>
                <ul class="list-group user-list">
                 ${userlist}
                </ul>`,
        title : `Enter room name and select users`  
    } 


    const userLiElement = (nickName) => [`<li data-user-nickname=${nickName} class='list-group-item'>${nickName}</li>`];

    const usersList =  (users) => {
        const publicLi = userLiElement("public");
        const userLiElements = users.map(user => userLiElement(user.nickName));
        return publicLi.concat(userLiElements).join("");
    }

    const users = usersList(userlist);
    const modal = Modal(modalconfig.title, modalconfig.content(users))
    const modalRendered = renderModal(modal);

    return  modalRendered;
};

export const CreateRoomModalState = (modal) => {

    const modalState = {
        modal : modal,
        name : null,
        users : [], 
        toggleUser (name) {
            if (this.users.includes(name)) {
                this.users = this.users.filter(user => user != name);
            }
            else if (name === "public") {
                this.users = ["public"];
            }
            else {
                this.users = this.users.filter(user => user != "public");
                this.users.push(name);
            }
            this.modal.find(".user-list li").each((index, user) => {
                if (this.users.includes(user.dataset.userNickname)) {
                    $(user).addClass("user-select");
                }
                else {
                    $(user).removeClass("user-select");
                }
            });
        }

    }
    
    const userSelect = e => modalState.toggleUser(e.target.dataset.userNickname);
    const nameSelect = e => modalState.name = e.target.value;

    modalState.modal.find(".user-list").on("click", userSelect)
    modalState.modal.find(".room-name").on("keyup", nameSelect)
    

    return {
        getUserNicknamesSelected : () => modalState.users,
        getNameRendered : () => modalState.name,
        onSaveShanges : (func) => {
            modalState.modal.find(".save-changes").on("click", func)
        }
    }
};













