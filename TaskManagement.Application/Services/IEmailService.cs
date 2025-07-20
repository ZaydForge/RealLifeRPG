﻿namespace TaskManagement.Application.Services;

public interface IEmailService
{
    Task<bool> SendOtpAsync(string toEmail, string otp);
}
