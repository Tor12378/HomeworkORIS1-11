using HttpServer.Attributes;
using HttpServer.Configuration;
using HttpServer.Models;
using HttpServer.MyORM;
using HttpServer.Services;

namespace HttpServer.Controllers;

[Controller("Account")]
public class AccountController
{
    private readonly MyDataContext _db = new("Server=(localdb)\\MSSQLLocalDB;Database=BattleDB");

    [Post("SendToEmail")]
    public static void SendToEmail(string email, string password)
    {
        new EmailSenderService(AppSettingsLoader.Instance()?.Configuration).SendEmail(email, password);
        Console.WriteLine("Email was sent successfully!");
    }

    [Get("Add")]
    public string Add(string login, string password)
    {
        _db.Add(new Account
        {
            Login = login,
            Password = password
        });

        return $"User {login} was added";
    }


    [Get("Delete")]
    public string Delete(string id)
    {
        var account = _db.SelectById<Account>(int.Parse(id));
        _db.Delete<Account>(int.Parse(id));

        return $"User {account.Login} was deleted";
    }

    [Get("Update")]
    public string Update(string id, string login, string password)
    {
        var account = _db.SelectById<Account>(int.Parse(id));

        var oldLogin = account.Login;

        account.Login = login;
        account.Password = password;

        _db.Update(account);

        return $"User {oldLogin} was updated";
    }

    [Get("GetAll")]
    public List<Account> GetAll() => _db.Select<Account>();

    [Get("GetById")]
    public Account GetById(string id) => _db.SelectById<Account>(int.Parse(id));
}