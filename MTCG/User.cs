namespace MTCG;

public class User
{
    public string Name { get; set; }
    private string _password;

    public string Password
    {
        get => _password;
        set => _password = value.ToLower();
    }

    public User(string name, string password)
    {
        this.Name = name;
        _password = password;
    }
}