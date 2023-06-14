public class User
{
    // public int idNumber;
    public string Email;    
    public string Username;
    // public string Password;
    
    public string FirstName;
    public string MiddleName;
    public string LastName;
    
    public int Age;
    public string Sex;

    public bool IsFaculty;
    public string ClassCode;
    // public bool IsEmailVerified;

    public User( string email, string username, string firstName, string middleName, string lastName, int age, string sex, bool isFaculty, string classCode)
    {
        // this.idNumber = idNumber;
        this.Email = email;
        this.Username = username;
        // this.Password = password;
       
        
        this.FirstName = firstName;
        this.MiddleName = middleName;
        this.LastName = lastName;
        
        this.Age = age;
        this.Sex = sex;

        this.IsFaculty = isFaculty;
        this.ClassCode = classCode;
        // this.IsEmailVerified = isEmailVerified;
    }
}
