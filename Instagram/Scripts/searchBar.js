
function searchForUser() {
    var x = document.getElementById("searchBar").value
    window.location.href = `/User/SearchPage/?SearchData=${x}`;
}