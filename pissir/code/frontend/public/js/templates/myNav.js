"use strict";

var myNavToggler = document.querySelector('.navbar-toggler');
var myNavbar = document.querySelector('.navbar-collapse');

myNavToggler.addEventListener('click', function() {
    myNavbar.classList.toggle('show');
});