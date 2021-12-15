using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace Rextester{
    public class Program{
        
        public static string getUserRole(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT role" + " from UserTable " + "WHERE UserTable.role = role", conn);
            cmd.ExecuteNonQuery();
            string role = "";
            role = Convert.ToString(cmd.ExecuteScalar());
            return role;
        }

        public static bool userExist(string username){
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(username)" + " from UserTable" + " WHERE UserTable.username = @username", conn);
            cmd.Parameters.AddWithValue("@username", userName);
            cmd.ExecuteNonQuery();
            int count = (int)cmd.ExecuteScalar();
            if(count > 0){
                return true;
            }
            else{
                return false;
            }
        }

        public static void updateUserRole(string username, string newRole){
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE UserTable" + " SET role = @newRole" + " WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("@newRole", newRole);
            cmd.Parameters.AddWithValue("@username", userName);
            cmd.ExecuteNonQuery();
        }
        
        // Main 
        public static void Main(string[] args)
        {
            string CurrentUsername = "abrio"
            if(getUserRole(CurrentUsername) == "Admin"){
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
                conn.Open();
                Console.WriteLine("Enter username to update account: ")
                string userSelected = Console.ReadLine();
                if(userExist(userSelected) == true){
                    if(getUserRole(userSelected) == "Admin"){
                        Console.WriteLine("Do you want to change " + userSelected + "' role to Student? Y/N")
                        string response = Console.ReadLine();
                        if(response == "Y"){
                            updateUserRole(userSelected, "Student")
                        }
                        else{
                            Console.WriteLine(userSelected + " will NOT be changed to Student")
                        }
                    }
                    if(getUserRole(userSelected) == "Student"){
                        Console.WriteLine("Do you want to change " + userSelected + "' role to Admin? Y/N")
                        string response = Console.ReadLine();
                        if(response == "Y"){
                            updateUserRole(userSelected, "Admin")
                        }
                        else{
                            Console.WriteLine(userSelected + " will NOT be changed to Admin")
                        }
                    }
                }
            }
               
        }
    }
}
