    $(document).ready(function(){
        var x = $("#presponse").text().trim();
        if (x.length > 0)
            $("#presponse").fadeIn(3500).delay(3500).fadeOut(1500);

        //for button register making it disbled
        var check = $("#check").prop("checked");
        if (check == 0) {
            $("#register").prop("disabled", true).css("cursor", "not-allowed");
        }
        //effect of button on selection of check box
        $("#check").change(function () {
            var check = $("#check").prop("checked");
            if (check == 0) {
                $("#register").prop("disabled", true).css("cursor", "not-allowed");
            }
            else {
                $("#register").prop("disabled", false).css("cursor", "pointer");
            }
        });

        //Validatoion
        $("#register").click(function () {
            var name, nameOfbusiness, phone, address, password, confirmPassword,result=true;
            name = $("#name").val().trim();
            nameOfBusiness = $("#nameOfBusiness").val().trim();
            phone = $("#phone").val().trim();
            email = $("#email").val().trim();
            address = $("#address").val().trim();
            password = $("#password").val().trim();
            confirmPassword = $("#confirmPassword").val().trim();
            $(".err").remove();

            //validating for name
            if (name.length == 0) {
                result = false;
                $("#name").after("<span class='text-warning err'>Enter Name.</span>");
            } else if (name.length < 2) {
                result = false;
                $("#name").after("<span class='text-warning err'>Enter a Valid Name.</span>");
            } /*else {
                for( var v of name)
                {   
                    //To find ascii value of each character in name
                    var n = name.charCodeAt(v);
                    if (n >= 48 && n <= 57) {
                        result = false;
                        $("#name").after("<span class='text-danger err'>No.are not allowed</span>");
                    }
                }
            }*/
            //validating for name of Business
            if (nameOfBusiness.length == 0) {
                result = false;
                $("#nameOfBusiness").after("<span class='text-warning err'>Enter Name Of Business.</span>");
            } else if (nameOfBusiness.length < 10) {
                result = false;
                $("#nameOfBusiness").after("<span class='text-warning err'>Business name should be more than 10 charcters.</span>");
            }
            //validating for Phone
            if (phone.length == 0) {
                result = false;
                $("#phone").after("<span class='text-warning err'>Enter Phone number.</span>");
            } else if (phone.length != 10) {
                result = false;
                $("#phone").after("<span class='text-warning err'>Enter a valid Phone number.</span>");
            } 
            //validating for email
            if (email.length == 0) {
                result = false;
                $("#email").after("<span class='text-warning err'>Enter Email.</span>");
            } else if (email.length < 12) {
                result = false;
                $("#email").after("<span class='text-warning err'>Enter a valid Email.</span>");
            }
            //validating for address
            if (address.length == 0) {
                result = false;
                $("#address").after("<span class='text-warning err'>Enter Address.</span>");
            } else if (address.length < 12) {
                result = false;
                $("#address").after("<span class='text-warning err'>Address should be more than 12 charcters.</span>");
            }
            //validating for password
            if (password.length == 0) {
                result = false;
                $("#password").after("<span class='text-warning err'>Enter Password.</span>");
            } else if (password.length < 8) {
                result = false;
                $("#password").after("<span class='text-warning err'>Password should be more than 8 charcters.</span>");
            }
            //validating for confirmPassword
            if (confirmPassword.length == 0) {
                result = false;
                $("#confirmPassword").after("<span class='text-warning err'>Enter Password.</span>");
            } else if (confirmPassword != password) {
                result = false;
                $("#confirmPassword").after("<span class='text-warning err'>Password doesn't match.</span>");
            }
            return result;
        });
    });