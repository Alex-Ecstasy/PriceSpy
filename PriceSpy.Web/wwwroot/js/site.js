// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function () {
    var input = document.querySelector("#search-box");
    var button = document.querySelector("#search-button");
    var rate = document.querySelector("#rate");

    button.onclick = function () {
        if (input.value) {
            location.href = "/?searchQuery=" + input.value + "&Rate=" + rate.value;
        }
    }
    input.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            document.querySelector("#search-button").click();
        }
    })
    rate.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            document.querySelector("#search-button").click();
        }
    })
});