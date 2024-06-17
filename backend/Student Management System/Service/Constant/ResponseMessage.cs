namespace Service.Constant
{
    public static class ResponseMessage
    {
        public const string UserNotFound = "No user found";
        public const string NoContent = "No content available";
        public const string InvalidEmailOrPassword = "Invalid email or password";
        public const string LoginSuccessful = "Login successful";
        public const string PasswordResetRequired = "Password reset required";
        public const string PasswordResetSuccessful = "Password reset successfully";
        public const string UserAddingSuccess = "User added successfully";
        public const string UserAddingFailure = "There is error to adding the user, please try again later.";
        public const string UserEditingSuccess = "User added successfully";
        public const string UserEditingFailure = "There is error to editing the user, please try again later.";
        public const string UserDeleteSuccess = "User deleted successfully.";
        public const string ValidationError = "Validation failed";
        public const string SomethingWentWrong = "Something went wrong, please try again later";
        public const string EmailAlreadyExists = "User with this email already exists, please try with another email";
        public const string TeacherAlreadyExists = "The teacher with this subject and class already exists";

        public const string EmailSendFailure = "There is error occurs while sending the email, please enter valid and existing email address";
        public const string RegisterEmailSubject = "Successfully Registered";
        public const string RegisterEmailBodyTemplate = "<html><body style='font-family: Arial, sans-serif;'>" +
            "<div style='border: 2px solid #007BFF; padding: 20px; background-color: #F0F8FF;'>" +
            "<h1 style='color: #007BFF; text-align: center;'>Welcome to Gurukul!</h1>" +
            "<p>Dear User,</p>" +
            "<p>You have successfully registered as <strong>{0}</strong>.</p>" +
            "<p>Your registration was completed by {1}.</p>" +
            "<p>You can now log in to the system using your password: <strong>{2}</strong>.</p>" +
            "<p>Thank you for joining Gurukul! We look forward to your active participation.</p>" +
            "<p>Regards,</p>" +
            "<p><em>Gurukul Schools</em></p>" +
            "</div>" +
            "</body></html>";


        public const string NewStudentNotiTeacherSubject = "New Student Enrolled";
        public const string NewStudentToTeacherBody = "<html><body style='font-family: Arial, sans-serif;'>" +
            "<div style='border: 2px solid #007BFF; padding: 20px; background-color: #F0F8FF;'>" +
            "<h1 style='color: #007BFF; text-align: center;'>Hello, Teachers!</h1>" +
            "<p>Dear Teachers,</p>" +
            "<p>We are pleased to inform you that a new student has been registered into your class: <strong>{0}</strong>.</p>" +
            "<p>This student will be joining your class from the next academic session.</p>" +
            "<p>Please extend a warm welcome and provide necessary guidance to help them settle in smoothly.</p>" +
            "<p>Regards,</p>" +
            "<p><em>Gurukul Schools</em></p>" +
            "</div>" +
            "</body></html>";


        public static readonly string AbsentEmailSubject = $"Student Absence Notification on Date {DateTime.UtcNow.Date:yyyy-MM-dd}";
        public const string AbsentEmailBodyTemplate = "<html><body>" +
         "<div style='border: 2px solid #000; padding: 20px;'>" +
         "<p>Dear Teachers,<br/><br/>This is to inform you that student {0} is absent today." + 
         "<p><br/><br/>Regards,<br/>{1}</p>" +
         "</div>" +
         "</body></html>";
        
        public static readonly string GradeEmailSubject = "Grade is added";
        public const string GradeEmailBodyTemplate = "<html><body>" +
         "<div style='border: 2px solid #000; padding: 20px;'>" +
         "<p>Dear Student,<br/><br/>This is to inform you that your grades for subject {0} is added." + 
         "<p>Please check it on Grade Dashboard." + 
         "<p><br/><br/>Regards,<br/>{1}</p>" +
         "</div>" +
         "</body></html>";

        public const string NoRecords = "No attendance records found";
        public const string NoGradeRecordsFound = "You haven't assigned any grade to anyone";
    }
}
