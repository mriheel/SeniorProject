﻿using System.Data;
using System.Data.SqlClient;
using StudentMultiTool.Backend.Services.DataAccess;

namespace StudentMultiTool.Backend.Models.Recipe
{
    public class RecipeDB
    {
        string connection = Environment.GetEnvironmentVariable(EnvironmentVariableEnum.CONNECTIONSTRING);

        public List<RecipeList> GetAllRecipe(int perPage, int page)
        {
            List<RecipeList> customerRecipe = new List<RecipeList>();

            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {

                    SqlCommand cmd = new SqlCommand("SELECT * FROM [recipes];", con);

                
                    con.Open();

                    try
                    {
                        SqlDataReader rd = cmd.ExecuteReader();

                        while (rd.Read())
                        {

                            customerRecipe.Add(new RecipeList
                            {
                                id = Convert.ToInt32(rd["id"]),
                                title = rd["title"].ToString(),
                                category = rd["category"].ToString(),
                                calorieValue = rd["calorieValue"].ToString(),
                                overallPrice = rd["overallPrice"].ToString(),
                                datePosted = rd["datePosted"].ToString(),
                                mealForPeople = Convert.ToInt32(rd["mealForPeople"]),
                                description = rd["description"].ToString()
                            });

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }


                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            int skip1 = perPage * page;
            int skip = skip1 - 4;


            if (customerRecipe.Count < skip1)
            {
                perPage = (customerRecipe.Count - skip);
            }
            return customerRecipe.ToList().GetRange(skip, perPage);
        }


        public RecipeList GetSingleRecipe(int id)
        {
            RecipeList oneRecipe = new RecipeList();

            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {

                    SqlCommand cmd = new SqlCommand("SELECT * FROM [recipes] where id =" + id + ";", con);

                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    try
                    {
                        SqlDataReader rd = cmd.ExecuteReader();

                        while (rd.Read())
                        {
                            oneRecipe.id = Convert.ToInt32(rd["id"]);
                            oneRecipe.title = rd["title"].ToString();
                            oneRecipe.category = rd["category"].ToString();
                            oneRecipe.calorieValue = rd["calorieValue"].ToString();
                            oneRecipe.overallPrice = rd["overallPrice"].ToString();
                            oneRecipe.datePosted = rd["datePosted"].ToString();
                            oneRecipe.mealForPeople = Convert.ToInt32(rd["mealForPeople"]);
                            oneRecipe.description = rd["description"].ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }


                    con.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return oneRecipe;
        }
        public bool addValue(RecipeList r)
        {
            bool result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    String query = "INSERT INTO [recipes] (title, category, calorieValue, overallPrice, datePosted, mealForPeople, description) VALUES (@ti, @ca, @cal, @price, @date, @meal, @desc);";

                    SqlCommand cmd = new SqlCommand(query, con);


                    try
                    {
                        con.Open();
                        //cmd.Parameters.AddWithValue("@id", r.id );//131);
                        cmd.Parameters.AddWithValue("@ti", r.title);// "Noodles");
                        cmd.Parameters.AddWithValue("@ca", r.category);//"launch");
                        cmd.Parameters.AddWithValue("@cal", r.calorieValue);//"80 calories");
                        cmd.Parameters.AddWithValue("@price", r.overallPrice);//"$8.0");
                        cmd.Parameters.AddWithValue("@date", r.datePosted);//"4/23/2022");
                        cmd.Parameters.AddWithValue("@meal", r.mealForPeople);//"2");
                        cmd.Parameters.AddWithValue("@desc", r.description);//"Yummmay delicious");
                        cmd.ExecuteNonQuery();
                        result = true;
                        con.Close();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = false;
            }

            return result;

        }

        public bool updateValue(int id, RecipeList r)
        {
            bool result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    String query = "UPDATE [recipes] SET title = @ti, category = @ca, calorieValue = @cal, overallPrice= @price, datePosted = @date, mealForPeople = @meal, description = @desc where id = " + id + ";";

                    SqlCommand cmd = new SqlCommand(query, con);


                    try
                    {
                        con.Open();
                        //cmd.Parameters.AddWithValue("@id", r.id);
                        cmd.Parameters.AddWithValue("@ti", r.title);
                        cmd.Parameters.AddWithValue("@ca", r.category);
                        cmd.Parameters.AddWithValue("@cal", r.calorieValue);
                        cmd.Parameters.AddWithValue("@price", r.overallPrice);
                        cmd.Parameters.AddWithValue("@date", r.datePosted);
                        cmd.Parameters.AddWithValue("@meal", r.mealForPeople);
                        cmd.Parameters.AddWithValue("@desc", r.description);
                        cmd.ExecuteNonQuery();
                        result = true;
                        con.Close();

                    }
                    catch (Exception e)
                    {
                        result = false;
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                Console.WriteLine(e);
            }

            return result;

        }

        public bool deleteValue(int id)
        {
            bool result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    String query = "DELETE FROM [recipes] where id = @id;";

                    SqlCommand cmd = new SqlCommand(query, con);

                    try
                    {
                        con.Open();
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        result = true;
                        con.Close();
                    }
                    catch (Exception e)
                    {
                        result = false;
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                Console.WriteLine(e);
            }
            return result;
        }
    }
}
