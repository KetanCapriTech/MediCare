function validateLoginForm() {

    let email = $("#Email").val().trim();
    let password = $("#Password").val().trim();
    if (email === "") {
        alert("Email is required");
        $("#Email").focus();
        return false;
    }

    if (password === "") {
        alert("Password is required");
        $("#Password").focus();
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
    $("#spinner").removeClass("d-none"); // SHOW spinner

    $.ajax({

        url: "/Auth/Register",
        type: "POST",
        data: $("#wdRegisterForm").serialize(),
        success: function (res) {
            $("#spinner").addClass("d-none");

            if (res.success) {
                window.location.href = "/Auth/ValidateOtp";
            }
            else {
                alert("something went wrong");
            }
        },
        error: function (xhr) {
            $("#spinner").addClass("d-none");
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
function sendValidateOtpRequest() {
    if ($("#Otp").val().trim() === "") {
        alert("OTP is required");
        return;
    }

    $("#spinner").removeClass("d-none");

    $.ajax({
        url: "/Auth/ValidateOtp",
        type: "POST",
        data: $("#MDVerifyOtpForm").serialize(),
        success: function (res) {
            $("#spinner").addClass("d-none");

            if (res.success) {
                window.location.href = "/Auth/ResetPassword";
            } else {
                alert(res.message || "Invalid OTP");
            }
        },
        error: function () {
            $("#spinner").addClass("d-none");
            alert("Server error");
        }
    });
}
function sendForgetPasswordRequest() {

    if ($("#Email").val().trim() === "") {
        alert("Otp is required");
        $("#Email").focus();
        return;
    }

    $("#spinner").removeClass("d-none");

    $.ajax({

        url: "/Auth/ForgotPassword",
        type: "POST",
        data: $("#MDForgetPasswordEmailForm").serialize(),

        success: function (res) {
            $("#spinner").addClass("d-none");

            if (res.success) {
                window.location.href = "/Auth/ValidateOtp";
            }
            else {
                alert(res.message || "Something went worng");
            }
        },
        error: function () {
            $("#spinner").addClass("d-none");
            alert("server error");
        }
    });
}
function sendResetPasswordRequest() {
    if ($("#Password").val().trim() === "") {
        alert("new Password is required");
        return;
    }
    if ($("#ConfirmPassword").val().trim() === "") {
        alert("Confirm Password is required");
        return;
    }
    if ($("#ConfirmPassword").val().trim() !== $("#Password").val().trim()) {
        alert("New Password and Confirm Password is not matched");
        return;
    }

    $("#spinner").removeClass("d-none");

    $.ajax({
        url: "/Auth/ResetPassword",
        type: "POST",
        data: $("#MDResetPaswordForm").serialize(),
        success: function (res) {
            $("#spinner").addClass("d-none");
            if (res.success) {
                window.location.href = "/Auth/Login";
            } else {
                alert(res.message || "Failed to reset password please try again letter");
            }
        },
        error: function () {
            $("#spinner").addClass("d-none");
            alert("Server error");
        }
    });
}

