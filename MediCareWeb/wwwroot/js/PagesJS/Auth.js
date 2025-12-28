function validateLoginForm() {

    let email = $("#email").val().trim();
    let password = $("#password").val().trim();
    if (email === "") {
        alert("Email is required");
        $("#email").focus();
        return false;
    }

    if (password === "") {
        alert("Password is required");
        $("#password").focus();
        return false;
    }

    return true;
}

function validateRegisterForm() {
    let firstName = $("#mdFirstName").val().trim();
    let lastName = $("#mdLastName").val().trim();
    let email = $("#mdEmail").val().trim();
    let password = $("#mdPassword").val().trim();
    let confirmPassword = $("#mdConfirmPassword").val().trim();
    let address = $("#mdAddress").val().trim();
    let role = $("#mdRole").val();

    if (firstName === "") {
        alert("First name is required");
        $("#mdFirstName").focus();
        return false;
    }

    if (lastName === "") {
        alert("Last name is required");
        $("#mdLastName").focus();
        return false;
    }

    if (email === "") {
        alert("Email is required");
        $("#mdEmail").focus();
        return false;
    }

    if (password === "") {
        alert("Password is required");
        $("#mdPassword").focus();
        return false;
    }

    if (confirmPassword === "") {
        alert("Confirm Password is required");
        $("#mdConfirmPassword").focus();
        return false;
    }

    if (address === "") {
        alert("Address is required");
        $("#mdAddress").focus();
        return false;
    }

    if (role === "0" || role === "" || role == null) {
        alert("Role is required");
        $("#mdRole").focus();
        return false;
    }

    return true;
}

function sendLoginRequest() {
    $("#spinner").removeClass("d-none");

    $.ajax({
        url: "/Auth/Login",
        type: "POST",
        data: $("#loginForm").serialize(),
        success: function (res) {
            $("#spinner").addClass("d-none");

            if (res.success) {
                window.location.href = "/Home/Index";
            } else {
                alert(res.message || "Invalid login");
            }
        },
        error: function () {
            $("#spinner").addClass("d-none");
            alert("Server error");
        }
    });
}

function sendregisterRequest() {
    $.ajax({

        url: "/Auth/Register",
        type: "POST",
        data: $("#wdRegisterForm").serialize(),
        success: function (res) {
            $("#spinner").addClass("d-none");

            if (res.success) {
                window.location.href = "/Auth/Login";
            }
            else {
                alert("something went wrong");
            }
        },
        error: function (xhr) {
            if (xhr.status === 409) {
                alert(xhr.responseJSON.message); 
            }
            else if (xhr.status === 202) {
                alert(xhr.responseJSON.message);
            }
            else {
                alert("Something went wrong");
            }
        }
    });
}