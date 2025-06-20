using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace ChatApplication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : Controller
    {
        [HttpPost]
        public IActionResult SendMessage([FromForm] long receiverID, [FromForm] string content)
        {
            // Use the instance-level connection here
            using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
            {
                try
                {
                    string insertSql = "INSERT INTO Message (ReceiverID, Content, SentTime, Delivered) VALUES (@ReceiverID, @Content, @SentTime, 0)";
                    using (SQLiteCommand cmd = new SQLiteCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ReceiverID", receiverID);
                        cmd.Parameters.AddWithValue("@Content", content);
                        cmd.Parameters.AddWithValue("@SentTime", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                        cmd.ExecuteNonQuery();
                    }
                    return Ok("Message sent successfully");
                }
                catch (Exception ex)
                {
                    // Log the exception details
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                    return StatusCode(500, "Internal server error");
                }
            }
        }

        [HttpGet]
        public IActionResult GetMessages([FromQuery] long receiverID)
        {
            var messages = new List<Messages>();

            try
            {
                using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                {
                    string selectSql = "SELECT MessageID, ReceiverID, Content, SentTime FROM Message WHERE ReceiverID = @ReceiverID AND Delivered = 0";

                    using (SQLiteCommand selectCmd = new SQLiteCommand(selectSql, connection))
                    {
                        selectCmd.Parameters.AddWithValue("@ReceiverID", receiverID);

                        using (var reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Messages data = new Messages
                                {
                                    MessageID = reader.GetInt32(0),
                                    ReceiverID = reader.GetInt32(1),
                                    Content = reader.GetString(2),
                                    SentTime = reader.GetInt32(3)
                                };
                                messages.Add(data);
                            }
                        }
                    }

                    // Mark messages as delivered
                    string updateSql = "UPDATE Message SET Delivered = 1 WHERE ReceiverID = @ReceiverID AND Delivered = 0";
                    using (SQLiteCommand updateCmd = new SQLiteCommand(updateSql, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@ReceiverID", receiverID);
                        updateCmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { messages });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult MarkMessagesAsDelivered([FromForm] long receiverID)
        {
            try
            {
                using (SQLiteConnection connection = DatabaseConnector.CreateNewConnection())
                {
                    string updateSql = "UPDATE Message SET Delivered = 1 WHERE ReceiverID = @ReceiverID AND Delivered = 0";
                    using (SQLiteCommand cmd = new SQLiteCommand(updateSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@ReceiverID", receiverID);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok("Messages marked as delivered");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}