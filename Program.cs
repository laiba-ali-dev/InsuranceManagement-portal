using Microsoft.EntityFrameworkCore;
using OnlineInsurance.DB;

namespace OnlineInsurance
{
	public class Program
	{
		public static void Main(string[] args)
		{   

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddSession();
			var provider = builder.Services.BuildServiceProvider();
			var config = provider.GetRequiredService<IConfiguration>;
			builder.Services.AddDbContext<mydbcontext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("mydbcontext")));

			var app = builder.Build();




			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseSession();
			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

            app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Admin}/{action=UserRegister}/{id?}");

			app.Run();

		}
	}
}