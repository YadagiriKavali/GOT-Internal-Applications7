// JavaScript Document
var homepgbanners = new Swiper('.homepgbanners', {
	pagination: '.swiper-pagination1',
	nextButton: '.swiper-button-next',
	prevButton: '.swiper-button-prev',
	paginationClickable: true,
	spaceBetween: 30,
	centeredSlides: true,
	autoplay: 2500,
	autoplayDisableOnInteraction: false
});

// Recent Activity
var recentactivity = new Swiper('.recentactivity', {
pagination: '.swiper-pagination2',
slidesPerView: 6,
//centeredSlides: true,
paginationClickable: true,
spaceBetween: 10,
breakpoints:{
1024:{
slidesPerView:6.5,
spaceBetween:10
},
768:{
slidesPerView:5.5,
spaceBetween:10,
},
640:{
slidesPerView:5.5,
spaceBetween:10
},
480:{
slidesPerView:4.5,
spaceBetween:10
},
375:{
slidesPerView:4.5,
spaceBetween:10
},
320:{
slidesPerView:3.5,
spaceBetween:10
}
	
}
});

// Recent Activity
var mostused = new Swiper('.mostused', {
pagination: '.swiper-pagination3',
slidesPerView: 6,
//centeredSlides: true,
paginationClickable: true,
spaceBetween: 10,
breakpoints:{
1024:{
slidesPerView:6.5,
spaceBetween:10
},
768:{
slidesPerView:5.5,
spaceBetween:10,
},
640:{
slidesPerView:5,
spaceBetween:10
},
480:{
slidesPerView:4.5,
spaceBetween:10
},
425:{
slidesPerView:4.5,
spaceBetween:10
},	
375:{
slidesPerView:4.5,
spaceBetween:10
},
320:{
slidesPerView:3.5,
spaceBetween:10
}	
}
});

//Page top scrolling
$(document).ready(function () {
$(window).scroll(function () {
if ($(this).scrollTop() > 100) {
$('.scrollup').fadeIn();
} else {
$('.scrollup').fadeOut();
}
});
$('.scrollup').click(function () {
$("html, body").animate({
scrollTop: 0
}, 600);
return false;
});
});



window.onload = function() {
  
  function setCurrentSlide(ele,index){
    $(".swiper1 .swiper-slide").removeClass("selected");
    ele.addClass("selected");
    //swiper1.initialSlide=index;
  }
  
  var swiper1 = new Swiper('.swiper1', {
        slidesPerView: 4,
        paginationClickable: true,
        spaceBetween: 0,
        freeMode: true,
        loop: false,
        onTab:function(swiper){
          var n = swiper1.clickedIndex;
          alert(1);
        }
    });
  swiper1.slides.each(function(index,val){
    var ele=$(this);
    ele.on("click",function(){
      setCurrentSlide(ele,index);
      swiper2.slideTo(index, 500, false);
      //mySwiper.initialSlide=index;
    });
  });
  
  
var swiper2 = new Swiper ('.swiper2', {
    direction: 'horizontal',
    loop: false,
    autoHeight: true,
    onSlideChangeEnd: function(swiper){
      var n=swiper.activeIndex;
      setCurrentSlide($(".swiper1 .swiper-slide").eq(n),n);
      swiper1.slideTo(n, 500, false);
    }
  });
//mySwiper.params.control = swiper;
//swiper.params.control = mySwiper;
    
}


