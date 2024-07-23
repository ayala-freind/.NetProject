using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BL.Interfaces;
using BL.Services;
using DAL.Data;
using DAL.Profilies;
using MODELS.Models;
using BL;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture);
            });

            // ����� ������� ����� �������
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ����� AutoMapper ����� ����
            builder.Services.AddSingleton<IMapper>(provider =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    // ����� �������� ���
                    cfg.AddProfile<CostumerProfile>();
                    cfg.AddProfile<ProductProfile>();
                });
                return config.CreateMapper();
            });

            //  DbContext ����� 
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ShopDB")));

            // ����� UserData ����� �������
            builder.Services.AddScoped<ProductData>();
            builder.Services.AddScoped<CostumerData>();

            // ����� IUserService �� ������ ��� UserService
            builder.Services.AddScoped<ICostumerService, CostumerService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();

            // ����� ����� �� ����� HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<JWTMiddleware>();
            app.UseMiddleware<PrintFunctionNameMiddleware>();

            //app.Run(async context =>
            //{
            //    context.Response.ContentType = "text/plain";
            //    await context.Response.WriteAsync("ok");
            //});

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
   
        }
    }
}