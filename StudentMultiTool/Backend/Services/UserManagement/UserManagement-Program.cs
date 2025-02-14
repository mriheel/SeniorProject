﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Data.SqlClient;
using System.Text;

namespace UserManagement
{
    public class UserManager
    {
        // TODO: Update password/passcode fields to accurately reflect database schema.
        // Checks if user exists, if not validates username and passcode are valid and creates new user
        public static bool CreateUsers(string name, string username, string email, string passcode, string school)
        {
            Update updateDB = new Update();
            bool userExist = Validate.UserExist(username);
            bool validUsername = false, validPasscode=false;
            if (userExist == false)
            {
                int length = username.Length;
                if (length >= 5)
                {
                    validUsername = Validate.ValidateUserName(username);
                }
                int passcodeLength = passcode.Length;
                if(passcodeLength >= 8)
                {
                    validPasscode = Validate.ValidatePassword(passcode);
                }
                if(validUsername == true && validPasscode == true)
                {
                    String token = Guid.NewGuid().ToString();
                    updateDB.UpdateCreate(username, email, passcode, school, token);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

            // Checks if user exists, and if they do deletes them
            public static bool DeleteUser(string username)
        {
            bool userExist = Validate.UserExist(username);
            if (userExist == true)
            {
                int temp;
                temp = Update.UpdateDelete(username);
                bool t = Evaluate.Eval(temp);
                if (t == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        // Checks if user exists, and if they do validates they are disabled, and enables user
        public static bool EnableUser(string username)
        {
            bool userExist = Validate.UserExist(username);
            if(userExist == true)
            {
                bool isDisabled = Validate.CheckDisabled(username);
                if (isDisabled == true)
                {
                    Update.UpdateEnable(username);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Checks if user exists, and if they do validates they are enabled, and disables user 
        public static bool DisableUser(string username)
        {
            bool userExist = Validate.UserExist(username);
            if (userExist == true)
            {
                bool isEnabled = Validate.CheckEnabled(username);
                if (isEnabled == true)
                {
                    Update.UpdateDisable(username);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Checks if user exists, and if they do validates their role, and updates user to new role
        public static bool UpdateRoleUser(string username, string role)
        {
            bool userExist = Validate.UserExist(username);
            if (userExist == true)
            {
                bool isUpdated = Validate.UserRoleCheck(username, role);
                if (isUpdated == true)
                {
                    Update.UpdateRole(username, role);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }


    }

    // Authorzies user is an admin 
    public class Authorize
    {
        static bool authorize(string username)
        
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT role" + " from UserAccounts " + "WHERE UserAccounts.role = role", conn);
            cmd.ExecuteNonQuery();
            string role = "";
            role = (string)cmd.ExecuteScalar();

            if (role == "admin" || role == "Admin")
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }

    public class Validate
    {
        // Checks if user exists in the database already
        public static bool UserExist(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd3 = new SqlCommand("SELECT COUNT (username)" + " from UserAccounts " + "WHERE UserAccounts.username = @username", conn);
            cmd3.Parameters.AddWithValue("@username", username);
            SqlDataReader reader = cmd3.ExecuteReader();
            int count = 1;
            reader.Close();
            count = (int)cmd3.ExecuteScalar();
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Checks is user is disabled (active status = 0)
        public static bool CheckDisabled(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT active_status" + " from UserAccounts" + " WHERE UserAccounts.username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
            bool active_status = (bool)cmd.ExecuteScalar();
            if (active_status == false)
            { 
                return true;
            }
            else 
            {
                return false;

            }

        }

        // Checks if user is enabled (active status =1)
        public static bool CheckEnabled(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT active_status" + " from UserAccounts" + " WHERE UserAccounts.username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
            bool active_status = (bool)cmd.ExecuteScalar();
            if (active_status == true)
            {
                return true;
            }
            else
            {
                return false;
            }
          
        }

        // Checks if user role is same as what trying to update to
        public static bool UserRoleCheck(string username, string role)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT COUNT (username)" + " from UserAccounts" + " WHERE UserAccounts.username = @username AND UserAccounts.role = @role", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@role", role);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Close();
            int count = (int)cmd.ExecuteScalar();
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Checks if username is valid 
        public static bool ValidateUserName(string username)
        {
            int validCondition = 0;
            foreach (char c in username)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validCondition++;
                   
                }
                else if (c >= '0' && c <= '9')
                {
                    validCondition++;
                }
                else
                {
                    char[] special = { '.', ',', '@', '!' };
                    if (username.IndexOfAny(special) == 1)
                    {
                        validCondition++;
                    }

                }
            }
            if(validCondition == username.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Checks if passcode is valid 
        public static bool ValidatePassword(string password)
        {
            int validCondition = 0;
            foreach (char c in password)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validCondition++;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    validCondition++;
                }
                else if (c >= '0' && c <= '9')
                {
                    validCondition++;
                    break;
                }
                else
                {
                    char[] special = { '.', ',', '@', '!', ' ' };
                    if (password.IndexOfAny(special) == 1)
                    {
                        validCondition++;
                    }

                }
            }
            if (validCondition == password.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

    public class Update
    {
        // TODO: Update password/passcode fields to accurately reflect database schema.
        // Updates database to delete user 
        public static int UpdateDelete(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM UserAccounts WHERE UserAccounts.username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
            SqlCommand cmd2 = new SqlCommand("SELECT COUNT (username)" + " from UserAccounts " + "WHERE UserAccounts.username = @username", conn);
            cmd2.Parameters.AddWithValue("@username", username);
            SqlDataReader reader = cmd2.ExecuteReader();
            int count = 1;
            reader.Close();
            count = (int)cmd2.ExecuteScalar();
            return count;
        }

        // Updates database to show user is enabled 
        public static void UpdateEnable(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE UserAccounts" + " SET active_status = @newStatus" + " WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("@newStatus", 1);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
         }

        // Updates database to show user is disabled 
        public static void UpdateDisable(string username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE UserAccounts" + " SET active_status = @newStatus" + " WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("@newStatus", 0);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
        }

        // Updates users role in the database
        public static void UpdateRole(string username, string newRole)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE UserAccounts" + " SET role = @role" + " WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("@role", newRole);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
        }

        bool isActive24Hour(string token)
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT emailCreated FROM UserAccounts" +
                                                " WHERE token = @token", conn);
                cmd.Parameters.AddWithValue("@token", token);
                cmd.ExecuteNonQuery();
                DateTime date = (DateTime)cmd.ExecuteScalar();
                conn.Close();
                DateTime now = DateTime.Now;
                if (date.AddDays(1) < now)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
        public bool ActivateAccount(string token)
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE UserAccounts" + " SET active_status = 1, verified_email = 1" + 
                                                " WHERE token = @token", conn);
                cmd.Parameters.AddWithValue("@token", token);
                cmd.ExecuteNonQuery();
                conn.Close();
                if (isActive24Hour(token))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch
            {
                return false;
            }
            
        }
        public bool UserTokenMatched(string username, string token)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT COUNT(username) FROM UserAccounts WHERE username = @username AND token = @token AND verified_email = 0", conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@token", token);
            cmd.ExecuteNonQuery();
            int numberOfMatchedAccounts = (int)cmd.ExecuteScalar();
            conn.Close();
            if (numberOfMatchedAccounts == 1)
            {
                return true;
            }
            else { return false; }
            
        }
        // Hashes passcode
        public string HashPass(string password)
        {

            string s = "teammarvel";
            byte[] salt = Encoding.ASCII.GetBytes(s);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public void UpdateCreate(string username, string email, string passcode, string school, string token)
        {
            // inserts the created user into the database
            passcode = HashPass(passcode);
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO UserAccounts (name, username,  email, passcode, role, school, active_status, token, verified_email) " +
                                                               "  values (@name, @username, @email, @passcode, @role, @school, @active_status, @token, @verified_email)", conn);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@name", "");
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@passcode", passcode);
            cmd.Parameters.AddWithValue("@role", "student");
            cmd.Parameters.AddWithValue("@school", school);
            cmd.Parameters.AddWithValue("@active_status", 0);
            cmd.Parameters.AddWithValue("@token", token);
            cmd.Parameters.AddWithValue("@verified_email", 0);
            cmd.ExecuteNonQuery();
            conn.Close();
            System.Console.WriteLine("New User Account created successfully.\n");
        }

        // Generates next ID number in database
        public static int generateID()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT MAX(userId) AS ID" + " FROM UserAccounts", conn);
            cmd.ExecuteNonQuery();
            int userId = (int)cmd.ExecuteScalar();
            if (userId != null)
            {
                return userId + 1;
            }
            else
            {
                return 1;
            }
        }



    }

    public class Evaluate
    {
        // Evaluates rows affected to a bool value 
        public static bool Eval(int rowsAffected)
        {
            if (rowsAffected == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // evaluates bool value to string.
        public static string Eval(bool affected)
        {
            if (affected == true)
            {
                string a = "Successful Changes.";
                return a;
            }
            else
            {
                string a = "Error";
                return a;
            }
        }
    }

    // Bulk Operations 
    public class BulkOperations
    {
        public static void BulkOps(string[] args)
        {

            string fileName = "bulkops.txt";
            string[] bulkOperLines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\" + fileName);
            for (int i = 0; i < bulkOperLines.Length; i++)
            {
                string[] singleOperation = bulkOperLines[i].Split(' '); // [0] = operation
                if (singleOperation[0] == "Create User")
                {
                    if (!Validate.UserExist(singleOperation[1]))
                    {
                        Console.WriteLine("User Created");
                    }
                }
                else if (singleOperation[0] == "Delete User")
                {
                    if (Validate.UserExist(singleOperation[1]))
                    {
                        Console.WriteLine("User deleted");
                    }
                }
                else if (singleOperation[0] == "Update Role")
                {
                    if (Validate.UserExist(singleOperation[1]))
                    {
                        Console.WriteLine("User's role updated");
                    }
                }
                else if (singleOperation[0] == "Enable User")
                {
                    if (Validate.UserExist(singleOperation[1]))
                    {
                        Console.WriteLine("User Enabled");
                    }
                }
                else if (singleOperation[0] == "Disable User")
                {
                    if (Validate.UserExist(singleOperation[1]))
                    {
                        Console.WriteLine("User Disabled");
                    }
                }

            }
        }
    }
}
