using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/index.html"));
SQLiteConnection connection = DatabaseConnector.Db();
SQLiteCommand command = connection.CreateCommand();
command.CommandText = "PRAGMA foreign_keys = ON;" +
    "CREATE TABLE if not Exists `User` " +
    "(`UserID` integer NOT NULL PRIMARY KEY, `UserName` text not NULL UNIQUE, `PasswordHash` text not NULL); " +
    "CREATE TABLE if not Exists `LoginAttempt`" +
    " (`AttemptID` integer NOT NULL PRIMARY KEY,  `SessionCookie` text NOT NULL UNIQUE, " +
    " `UserID` integer not null, " +
    "`AttemptTime` integer not null, `Success` integer not null, " +
    "FOREIGN KEY (`UserID`) REFERENCES `User`(`UserID`));" +
    "CREATE TABLE if not Exists `Message` (`MessageID` integer NOT NULL PRIMARY KEY, " +
    "`SenderID` integer NOT NULL, `ReceiverID` integer NOT NULL, " +
    "`Content` text NOT NULL, `SentTime` integer NOT NULL, `Delivered` integer NOT NULL DEFAULT 0, " +
    "FOREIGN KEY (`ReceiverID`) REFERENCES `User`(`UserID`));";
command.ExecuteNonQuery();
command.Dispose();
app.Run();