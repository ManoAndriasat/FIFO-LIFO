function showHide() {
  var passwordInput = document.getElementById("pwd");
  var checkbox = document.getElementById("Check");

  if (checkbox.checked) {
    passwordInput.type = "text";
  } else {
    passwordInput.type = "password";
  }
}