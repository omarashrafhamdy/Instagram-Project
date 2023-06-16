function showpass() {
    var x = document.getElementById("form10");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}
