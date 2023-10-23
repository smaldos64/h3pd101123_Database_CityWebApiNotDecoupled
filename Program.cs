
using CityWebApiNotDecoupled.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CityWebApiNotDecoupled
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddControllers().AddJsonOptions(x =>
            //                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // LTPE

            builder.Services.AddControllers();

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiContext")));

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}