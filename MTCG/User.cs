namespace MTCG;

public class User
{
    public string username;
    private string _password;

    public string Username
    {
        get
        {
            return this.username;
        }
        set
        {
            // Later on use SQL to check uniqueness of username
            if (value.Length == 0)
            {
                throw new Exception("Username cannot be empty.");
            }
            this.username = value;
        }
    }

    public string Password
    {
        get
        {
            return this._password;
        }
        set
        {
            if (value.Length < 8)
            {
                throw new Exception("Password cannot be less than 8 characters.");
            }
            this._password = value;
        }
    }

    public User(string name, string password)
    {
        this.Username = name;
        _password = password;

}