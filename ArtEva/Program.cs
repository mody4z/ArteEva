
using ArteEva.Data;
using ArteEva.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtEva
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(
                options =>
                    options.SuppressModelStateInvalidFilter = true);
            //inject Dbcontext
            builder.Services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("ArtCs"));
                });
            //regester identity
            builder.Services.AddIdentity<User, Role>()
               .AddEntityFrameworkStores<ApplicationDbContext>();


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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
