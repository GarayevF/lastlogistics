let nav_burger_menu = document.getElementById("nav_burger_menu");
let burgerMenuContainer = document.getElementById("nav_burger_dropdown_menu");

nav_burger_menu.addEventListener("click", activeBurgerMenu)
burgerMenuContainer.addEventListener("click", activeBurgerSubMenu)

function activeBurgerMenu() {
  burgerMenuContainer.classList.toggle("nav_burger_dropdown_menu_active")
}

function activeBurgerSubMenu(e) {
  let isActive = e.target.firstElementChild.classList.value.includes("nav_burger_dropdown_submenu_active");
  if (e.target.classList.value == "nav_burger_dropdown_menuitem") {
    if (!isActive) {
      isActive = true;
      e.target.firstElementChild.classList.add("nav_burger_dropdown_submenu_active");
      let height = e.target.firstElementChild.scrollHeight;
      e.target.firstElementChild.style.height = `${height}px `;
      e.target.firstElementChild.style.opacity = `1`;

    } else {
      isActive = false;
      e.target.firstElementChild.classList.remove("nav_burger_dropdown_submenu_active");
      
      setTimeout(() => {
        e.target.firstElementChild.style.height = 0;
      }, 100)
      setTimeout(() => {
        e.target.firstElementChild.style.opacity = 0;
      }, 150)
    }

  }

}
