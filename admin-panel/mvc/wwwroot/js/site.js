// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var superData = ""
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

function process_restaurants(data) {
    res = {}
    res.results = data.items.map((item) => {
        return {
            id: item.id,
            text: item.name
        }
    })
    res.pagination = {
        more: data.pageInfo.size > data.pageInfo.rangeEnd
    }
        
    
    console.log(res)
    console.log(data)
    superData = res
    return res;
}