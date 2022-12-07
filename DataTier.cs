namespace FinalProject;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
class DataTier
{
    public string connStr = "server=20.172.0.16;user=mooladimeji1;database=mooladimeji1;port=8080;password=mooladimeji1";

    // perform login check using Stored Procedure "LoginCheck" 
    public bool LoginCheck(User user)
    {
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            string procedure = "LoginCheck";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure; // set the commandType as storedProcedure
            cmd.Parameters.AddWithValue("@inputofficer_username", user.userName);
            cmd.Parameters.AddWithValue("@inputofficer_password", user.userPassword);
            cmd.Parameters.Add("@officerCount", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            MySqlDataReader rdr = cmd.ExecuteReader();

            int returnCount = (int)cmd.Parameters["@officerCount"].Value;
            rdr.Close();
            conn.Close();

            if (returnCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return false;
        }

    }
    // 
    public bool PackageCheck(string full_name, int unit_number)
    {
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            string procedure = "CheckResident";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters.Add("@residentCount", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            MySqlDataReader rdr = cmd.ExecuteReader();

            int returnCount = (int)cmd.Parameters["@residentCount"].Value;
            rdr.Close();
            conn.Close();

            if (returnCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return false;
        }

    }
    // Add Packages
    public void AddToPendingArea(int unit_number, string full_name, string posting_agency)
    {
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            string procedure = "AddToPendingArea";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters["@inputunit_number"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters["@inputfull_name"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputposting_agency", posting_agency);
            cmd.Parameters["@inputposting_agency"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
        }

    }

    public DataTable ShowAllPending(User user)
    {
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            string procedure = "ShowAllPending";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tableAllPending = new DataTable();
            tableAllPending.Load(rdr);
            rdr.Close();
            conn.Close();
            return tableAllPending;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null;
        }
    }
    public void AddToPackageHistory(int unit_number, string full_name, string posting_agency)
    {
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            string procedure = "AddToPackageHistory";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters["@inputunit_number"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters["@inputfull_name"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputposting_agency", posting_agency);
            cmd.Parameters["@inputposting_agency"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
        }

    }

    public DataTable ShowHistory(User user)
    {
        MySqlConnection conn = new MySqlConnection(connStr);
        Console.WriteLine("Input the unit number");
        int unit_number = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("Input Resident name");
        string full_name = Console.ReadLine();

        try
        {
            conn.Open();
            string procedure = "ShowHistory";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters["@inputunit_number"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters["@inputfull_name"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tableRecords = new DataTable();
            tableRecords.Load(rdr);
            rdr.Close();
            conn.Close();
            return tableRecords;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null;
        }

    }

    public DataTable ShowPending(User user, int unit_number, string full_name)
    {
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            string procedure = "ShowPending";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters["@inputunit_number"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters["@inputfull_name"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tableRecords = new DataTable();
            tableRecords.Load(rdr);
            rdr.Close();
            conn.Close();
            return tableRecords;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null;
        }

    }

    public void DeleteFromPending(int unit_number, string full_name)
    {
        MySqlConnection conn = new MySqlConnection(connStr);
        try
        {
            conn.Open();
            string procedure = "RemovePackage";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters["@inputunit_number"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters["@inputfull_name"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
        }

    }

    // To convert string
    public string ResidentEmail(int unit_number, string full_name)
    {
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            string procedure = "ResidentEmail";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputunit_number", unit_number);
            cmd.Parameters["@inputunit_number"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputfull_name", full_name);
            cmd.Parameters["@inputfull_name"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();
            string email = string.Empty;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    email = rdr.GetString(0);
                    break;
                }
            }
            else
            {
                Console.WriteLine("No Data Found Here.");
            }
            rdr.Close();

            return email;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null;
        }

    }

    // Add package to Unknown Area
    public DataTable AddToUnknownArea(string packageOwner, string postServiceAgency, string deliveryDate)
    {
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            string procedure = "AddToUnknownArea";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inputowner_name", packageOwner);
            cmd.Parameters["@inputowner_name"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputposting_agency", postServiceAgency);
            cmd.Parameters["@inputposting_agency"].Direction = ParameterDirection.Input;
            cmd.Parameters.AddWithValue("@inputdelivery_date", deliveryDate);
            cmd.Parameters["@inputdelivery_date"].Direction = ParameterDirection.Input;

            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tableRecords = new DataTable();
            tableRecords.Load(rdr);
            rdr.Close();
            conn.Close();
            return tableRecords;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null;
        }

    }

    public DataTable ShowUnknownPackage(User user)
    {
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            string procedure = "ShowUnknownPackage";
            MySqlCommand cmd = new MySqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;


            MySqlDataReader rdr = cmd.ExecuteReader();

            DataTable tableRecords = new DataTable();
            tableRecords.Load(rdr);
            rdr.Close();
            conn.Close();
            return tableRecords;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            conn.Close();
            return null;
        }

    }
}
