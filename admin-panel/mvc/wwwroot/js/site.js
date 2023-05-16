// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function show_edit_field(event) {
    event.preventDefault()
    let target = event.target
    target.onclick = null
    target.previousElementSibling.removeAttribute("hidden")
    target.setAttribute("type", "submit")
    console.log(event.target.previousElementSibling)
}

function delete_confirm(event) {
    event.preventDefault()
    let target = event.target
    console.log(target)
    target.onclick = null
    target.innerText = "Точно?"
    target.setAttribute("type", "submit")
}