using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OnlineInsurance.Models;

namespace OnlineInsurance.DB
{
	public class mydbcontext : DbContext
	{

		public mydbcontext()
		{ 
		}
		public mydbcontext(DbContextOptions<mydbcontext> options) : base(options)
		{

		}
		public virtual DbSet<Admin> admins { get; set; }

        public virtual DbSet<Employee> employees { get; set; }

	   public virtual DbSet<User> users { get; set; }

        public virtual DbSet<Insurance> insurances { get; set; }

		public virtual DbSet<Home> homes { get; set; }

		public virtual DbSet<Motor> motors { get; set; }

		public virtual DbSet<Life> lifes { get; set; }

		public virtual DbSet<Health> healths { get; set; }

		public virtual DbSet<InsuranceRequest> insurancerequests { get; set; }
    }
}
