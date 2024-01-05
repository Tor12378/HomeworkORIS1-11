﻿using HttpServer.Attributes;
using HttpServer.Configuration;
using HttpServer.Models;
using HttpServer.Services;

namespace HttpServer.Controllers;

[Controller("Account")]
public class AccountControllers
{
    private static readonly List<Account> Accounts = new()
    {
        new Account
        {
            Id = 0,
            Login = "Noname",
            Password = "qwerty"
        }
    };

    [Post("SendToEmail")]
    public static void SendToEmail(string email, string password)
    {
        new EmailSenderService(AppSettingsLoader.Instance()?.Configuration).SendEmail(email, password);
        Console.WriteLine("Email was sent successfully!");
    }

    [Get("Add")]
    public string Add(string login, string password)
    {
        Accounts.Add(new Account
        {
            Id = Accounts.Max(x => x.Id) + 1,
            Login = login,
            Password = password
        });

        return $"User {login} was added";
    }

    [Get("Delete")]
    public string Delete(string id)
    {
        var entity = Accounts.FirstOrDefault(x => x.Id == uint.Parse(id));

        if (entity is null)
            return $"User with Id:{id} was not found";

        Accounts.Remove(entity);
        
        return $"User {entity.Login} was deleted";
    }

    [Get("Update")]
    public string Update(string id, string login, string password)
    {
        var account = Accounts.FirstOrDefault(x => x.Id == int.Parse(id));

        if (account is null)
            return $"User with Id:{id} was not found";

        Accounts.Remove(account);

        account.Login = login;
        account.Password = password;

        Accounts.Add(account);
        
        return $"User {account.Login} was updated";
    }

    [Get("GetAll")]
    public List<Account> GetAll() => Accounts;

    [Get("GetById")]
    public Account GetById(string id) => Accounts.FirstOrDefault(x => x.Id == uint.Parse(id)) ?? new Account();
}