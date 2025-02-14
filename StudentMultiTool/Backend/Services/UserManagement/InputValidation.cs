﻿using System.Data.SqlClient;
using System.Text;

namespace StudentMultiTool.Backend.Services.UserManagement
{
    public class InputValidation
    {
        public InputValidation() { }

        // email must contaion @ and .edu extension
        public bool validateEmail(string email)
        {
            if (email.Contains("@") && email.Count(f => (f == '@')) == 1)
            {
                if (email.Length > 4 && email.Substring(email.Length - 4) == ".edu")
                {
                    return true;
                }
                    
            }
            System.Console.WriteLine("Invalid email address. It must be a valid student email address. Try again!");
            return false;
        }
        public bool emailExists(string email)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Environment.GetEnvironmentVariable("MARVELCONNECTIONSTRING");
            conn.Open();
            SqlCommand cmd3 = new SqlCommand("SELECT COUNT (email)" + " from UserAccounts " + "WHERE UserAccounts.email = @email", conn);
            cmd3.Parameters.AddWithValue("@email", email);
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

        // password must be 8 or more characters, but it can include integers and uppercase and lowercase characters
        public bool validatePasscode(string passcode)
        {
            bool validPasscode = true;
            if (passcode.Length >= 5 ) //8)
            {

                byte[] asciiInput = Encoding.ASCII.GetBytes(passcode);
                foreach (byte element in asciiInput)
                {
                    if (element < 48 && element != 32)
                    {
                        validPasscode = false;
                    }
                    else if (element > 57 && element < 64)
                    {
                        validPasscode = false;
                    }
                    else if (element > 90 && element < 97)
                    {
                        validPasscode = false;
                    }
                    else if (element > 122)
                    {
                        validPasscode = false;
                    }
                }

            }
            else
            {
                validPasscode = false;
            }
            if (!validPasscode)
            {
                System.Console.WriteLine("Invalid passcode. Try again!");
            }
            return validPasscode;
        }

        // username must be 8 or more characters, and at least one lowercase characters and a number
        public bool validateUsername(string username)
        {
            bool validUsername = true;
            if (username.Length >= 8)
            {
                byte[] asciiInput = Encoding.ASCII.GetBytes(username);
                foreach (byte element in asciiInput)
                {
                    if (element < 48)
                    {
                        validUsername = false;
                    }
                    else if (element > 57 && element < 65)
                    {
                        if (element != 64)
                        {
                            validUsername = false;
                        }
                    }
                    else if (element > 90 && element < 97)
                    {
                        validUsername = false;
                    }
                    else if (element > 122)
                    {
                        validUsername = false;
                    }
                }

            }
            else
            {
                validUsername = false;
            }
            if (!validUsername)
            {
                System.Console.WriteLine("Invalid username. Username must be 8 or more characters long. Try again!");
            }
            return validUsername;
        }

        public bool usernameExists(string username)
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

        public bool validateSchool(string school)
        {
            school = school.ToString();
            if (school.Length < 2)
            {
                System.Console.WriteLine("Invalid school. Try again!");
                return false;
            }
            return true;
        }
    }
}
