﻿namespace HttpServer.services;

public interface IEmailSenderService
{
    public void SendEmail(string login, string password);
}