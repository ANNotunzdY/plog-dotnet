using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure メソッドでアプリケーションの設定を行います
app.Use((HttpContext context, Func<Task> next) =>
{
    // カスタムヘッダーを追加
    context.Response.Headers.Append("X-PLog-API-Version", "1.0");
    context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

    // 次のミドルウェアに処理を渡します
    return next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var connectionString = "Data Source=plog.db";

app.MapGet("/", () =>
{
    var result = new List<List<object>>();
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var sql = "SELECT DATE, TENPO, KISHU, KEKKA FROM HISTORIES;";
        using (var command = new SqliteCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add([
                        (string)reader["DATE"],
                        (string)reader["TENPO"],
                        (string)reader["KISHU"],
                        (Int64)reader["KEKKA"]
                    ]);
                }
                reader.Close();
            }
        }
        connection.Close();
    }
    return result;
})
.WithName("Histories")
.WithOpenApi();

app.MapPost("/register", (string date, string tenpo, string kishu, Int64 kekka) =>
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var sql = "INSERT INTO HISTORIES (DATE, TENPO, KISHU, KEKKA) VALUES (@date, @tenpo, @kishu, @kekka);";
        using (var command = new SqliteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@tenpo", tenpo);
            command.Parameters.AddWithValue("@kishu", kishu);
            command.Parameters.AddWithValue("@kekka", kekka);
            command.ExecuteNonQuery();
        }
        connection.Close();
        return "OK";
    }
})
.WithName("Register")
.WithOpenApi();

app.Run();

