namespace FinalProject;
using System.Data;
using MySql.Data.MySqlClient;
class GuiTier{
    User user = new User();
    DataTier database = new DataTier();


    // Login page
    public User Login()
    {
        Console.WriteLine("------VacLife Apartment Package Management System------");
        Console.WriteLine("Please input userName (officer_username): ");
       user.userName = Console.ReadLine();
        Console.WriteLine("Please input the userPassword: ");
        user.userPassword = Console.ReadLine();
        return user;
    }
    // print Dashboard after user logs in successfully
    public int Dashboard(User user)
    {
        DateTime localDate = DateTime.Now;
        Console.WriteLine("---------------VacLife Dashboard-------------------");
        Console.WriteLine($"Hello: {user.userName}; Date/Time: {localDate.ToString()}");
        Console.WriteLine("To continue, please select one of the options below:");
        Console.WriteLine("1. Add a Package");
        Console.WriteLine("2. Package Status");
        Console.WriteLine("3. Check Package History");
        Console.WriteLine("4. Log Out");
        int option = Convert.ToInt16(Console.ReadLine());
        return option;
    }

    // Package Record History
    public void Display(DataTable tableRecords)
    {
        Console.WriteLine("---------------Table-------------------");
        foreach (DataRow row in tableRecords.Rows)
        {
            Console.WriteLine($"UnitNumber: {row["unit_number"]} \t Resident_Name: {row["full_name"]} \t Agency:{row["posting_agency"]}");
        }
    }
    public void DisplayUnknownArea(DataTable tableRecords)
    {
        Console.WriteLine("---------------Table-------------------");
        foreach (DataRow row in tableRecords.Rows)
        {
            Console.WriteLine($"owner_name: {row["owner_name"]} \t Posting_agency: {row["posting_agency"]} \t Delivery_date:{row["delivery_date"]}");
        }
    }
}

