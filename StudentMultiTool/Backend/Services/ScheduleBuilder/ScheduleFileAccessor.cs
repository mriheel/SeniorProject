﻿using System.Text.Json;
using System.Text.Json.Nodes;
using StudentMultiTool.Backend.Services.DataAccess;

namespace StudentMultiTool.Backend.Models.ScheduleBuilder
{
    // Represents a user's schedule.
    public class ScheduleFileAccessor
    {
        public static string Success { get; } = "Success";
        public bool Indentation { get; set; }
        public ScheduleFileAccessor(bool Indentation = false)
        {
            this.Indentation = Indentation;
        }

        // Writes all ScheduleItems in a given Schedule to a .json file.
        // The "schedule" argument is simply the Schedule whose ScheduleItems
        // are written to the file. The file path is obtained from the Schedule's
        // Path property.
        // The "indented" argument determines whether or not the resulting
        // file should be indented. Indentation should only be used for demonstrative
        // or testing purposes. When deployed, indentation should not be used, to save
        // storage space.
        public string WriteScheduleItems(Schedule schedule, string basePath)
        {
            try
            {
                string result = "";
                using (FileStream stream = File.Create(basePath + schedule.Path))
                {
                    // Configure writer to indent the .json file, or not.
                    JsonWriterOptions options = new JsonWriterOptions();
                    options.Indented = this.Indentation;

                    using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, options))
                    {
                        // Convert the schedule to a JsonObject
                        // Each ScheduleItem will be automatically converted
                        // to a JsonObject as well, and added to a JsonArray
                        // If there are no ScheduleItems, an empty JsonArray
                        // will be written to the file.
                        JsonObject scheduleAsJson = schedule.ToJson();

                        try
                        {
                            scheduleAsJson.WriteTo(writer);
                            result = ScheduleFileAccessor.Success;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            result = ex.Message;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
        // Overload for writing JsonArray instead of from a Schedule
        public string WriteScheduleItems(string path, JsonArray items, string basePath)
        {
            try
            {
                string result = "";
                using (FileStream stream = File.Create(basePath + path))
                {
                    // Configure writer to indent the .json file, or not.
                    JsonWriterOptions options = new JsonWriterOptions();
                    options.Indented = this.Indentation;

                    using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, options))
                    {
                        // Convert the items to a JsonObject to utilize JsonObject.WriteTo
                        // Each ScheduleItem will be automatically converted
                        // to a JsonObject as well, and added to a JsonArray
                        // If there are no ScheduleItems, an empty JsonArray
                        // will be written to the file.
                        JsonObject itemsAsJson = new JsonObject();
                        itemsAsJson[ScheduleItemOptions.JsonArrayCount] = items.Count;
                        itemsAsJson[ScheduleItemOptions.JsonArrayName] = items;

                        try
                        {
                            itemsAsJson.WriteTo(writer);
                            result = ScheduleFileAccessor.Success;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            result = ex.Message;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        // Reads all ScheduleItems in a given .json file.
        public List<ScheduleItem> ReadScheduleItems(string path)
        {
            Console.WriteLine("ScheduleFileAccessor.ReadScheduleItems: " + path);
            // Set up the List to store the results
            List<ScheduleItem> result = new List<ScheduleItem>();

            // Check that the file at the specified path actually exists
            // If it doesn't, just return an empty list
            if (!File.Exists(path))
            {
                return result;
            }

            //// Check that the file extension is "json". This isn't foolproof
            //// in terms of security but the file needs to be a .json for
            //// the ScheduleFileAccessor to work.
            string extension = Path.GetExtension(path.ToLower()).ToLower();
            if (!extension.Equals(".json"))
            {
                return result;
            }

            try
            {
                // Get the contents of the file as a string
                string contents = File.ReadAllText(path);

                // Parse the contents
                JsonNode? jsonContents = JsonNode.Parse(contents)!;

                // If the contents are null, return the empty list
                if (jsonContents == null)
                {
                    return result;
                }
                
                // This will be used to update each ScheduleItem's Id property
                int count = 0;

                // If the contents weren't null, try to get the scheduleItems element
                // from the json contents
                // This needs to be cast as an array to read all of the ScheduleItems
                // from it
                JsonArray itemsArray = (JsonArray) jsonContents![ScheduleItemOptions.JsonArrayName]!;
                foreach(JsonNode? currentNode in itemsArray)
                {
                    // The currentNode can only be read if it isn't null
                    // If it is null, it will just be skipped
                    if (currentNode != null)
                    {
                        // Instantiate a new ScheduleItem with Creator == 0
                        ScheduleItem currentItem = new ScheduleItem(count, 0);

                        // Update the Creator property
                        currentItem.Creator = (int) currentNode![ScheduleItemOptions.JsonCreator]!;

                        // Set the Title, Location, Contact, and Notes
                        currentItem.Title = (string) currentNode![ScheduleItemOptions.JsonTitle]!;
                        currentItem.Location = (string) currentNode![ScheduleItemOptions.JsonLocation]!;
                        currentItem.Contact = (string) currentNode![ScheduleItemOptions.JsonContact]!;
                        currentItem.Notes = (string) currentNode![ScheduleItemOptions.JsonNotes]!;

                        // Set the days of the week
                        currentItem.DaysOfWeek = new List<bool>
                        {
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonSunday]!,
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonMonday]!,
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonTuesday]!,
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonWednesday]!,
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonThursday]!,
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonFriday]!,
                            (bool) currentNode![ScheduleItemOptions.JsonDays]![ScheduleItemOptions.JsonSaturday]!
                        };

                        // Set the start and end times
                        currentItem.StartTime = new TimeOnly
                        (
                            (int) currentNode![ScheduleItemOptions.JsonStart]![ScheduleItemOptions.JsonHour]!,
                            (int) currentNode![ScheduleItemOptions.JsonStart]![ScheduleItemOptions.JsonMinute]!
                        );
                        currentItem.EndTime = new TimeOnly
                        (
                            (int) currentNode![ScheduleItemOptions.JsonEnd]![ScheduleItemOptions.JsonHour]!,
                            (int) currentNode![ScheduleItemOptions.JsonEnd]![ScheduleItemOptions.JsonMinute]!
                        );

                        // Add the unpacked ScheduleItem to the results
                        result.Add(currentItem);

                        // Increment count to use in the next ScheduleItem
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}
