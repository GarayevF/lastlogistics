let childrenLength =
    document.getElementsByClassName("swiper-wrapper")[0].children.length;
let count = 0;
if (childrenLength % 2 == 0) {
    count = childrenLength / 2;
} else {
    count = Math.round(childrenLength / 2 - 1);
}
const swiper = new Swiper(".mySwiper", {
    slidesPerView: 6,
    //spaceBetween: 5,
    freeMode: true,
    loop: true,
    autoplay: {
        delay: 1500,
        disableOnInteraction: false,
    },
    breakpoints: {
        // when window width is <= 499px
        300: {
            slidesPerView: count,
        },
        448: {
            slidesPerView: count,
        },
        // when window width is <= 999px
        768: {
            slidesPerView: count,
        },
        1024: {
            slidesPerView: count,
        },

        1290: {
            slidesPerView: count,
        },
    },
    navigation: {
        nextEl: ".swiper-next",
        prevEl: ".swiper-prev",
    },
});
