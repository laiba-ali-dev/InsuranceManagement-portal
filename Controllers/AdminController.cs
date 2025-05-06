using Microsoft.AspNetCore.Mvc;
using OnlineInsurance.DB;
using OnlineInsurance.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OnlineInsurance.Controllers
{
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly mydbcontext context;
        IWebHostEnvironment env;

        public AdminController(ILogger<AdminController> logger, mydbcontext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;

        }


        //ADMIN PANEL 


        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(Admin user)
        {
            if (ModelState.IsValid)
            {
                await context.admins.AddAsync(user);
                await context.SaveChangesAsync();
                TempData["Success"] = "Registered Successfully";
                return RedirectToAction("Login");

            }
            return View();
        }


        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                return RedirectToAction("Dashboard");

            }
            return View();

        }


        [HttpPost]
        public IActionResult Login(Admin user)
        {
            var Myuser = context.admins.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
            if (Myuser != null)
            {
                HttpContext.Session.SetString("UserSession", Myuser.Email);
                HttpContext.Session.SetString("UsernameSession", Myuser.Name);
                HttpContext.Session.SetString("UserId", Myuser.Id.ToString());


                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed..";
            }
            return View();
        }





        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mysession = HttpContext.Session.GetString("UserSession").ToString();
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();
                ViewBag.Myidsession = HttpContext.Session.GetString("UserId").ToString();



            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                HttpContext.Session.Remove("UserSession");
                HttpContext.Session.Remove("UsernameSession");
                HttpContext.Session.Remove("UserId");

                return RedirectToAction("Login");

            }
            return View();
        }


        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                var data = context.employees.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        public IActionResult AddEmployee()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AddEmployee(EmployeeViewModel emp)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();
                return RedirectToAction("Login");
            }

            string fileName = "";
            if (emp.photo != null)
            {
                var ext = Path.GetExtension(emp.photo.FileName);
                var size = emp.photo.Length;
                if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
                {

                    if (size <= 1000000)//1 MB
                    {

                        string folder = Path.Combine(env.WebRootPath, "images");
                        fileName = Guid.NewGuid().ToString() + "_" + emp.photo.FileName;
                        string filePath = Path.Combine(folder, fileName);
                        emp.photo.CopyTo(new FileStream(filePath, FileMode.Create));

                        Employee p = new Employee()
                        {
                            Name = emp.Name,
                            Email = emp.Email,
                            Phone = emp.Phone,
                            Password = emp.Password,
                            Image_path = fileName
                        };
                        context.employees.Add(p);
                        context.SaveChanges();
                        TempData["Success"] = "Employee Added..";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Size_Error"] = "Image must be less than 1 MB";
                    }
                }
                else
                {
                    TempData["Ext_Error"] = "Only PNG, JPG, JPEG iamges allowed";
                }

            }

            return View();
        }



        public async Task<IActionResult> Edit(int? Id)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();
                if (Id == null || context.employees == null)
                {
                    return NotFound();
                }
                var data = await context.employees.FindAsync(Id);
                if (data == null)
                {
                    return NotFound();
                }
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? Id, Employee emp)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();
                return RedirectToAction("Login");
            }

            if (Id != emp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                context.employees.Update(emp);
                TempData["update_success"] = "updated..";
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }


        public async Task<IActionResult> Delete(int? Id)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                if (Id == null || context.employees == null)
                {
                    return NotFound();
                }
                var data = await context.employees.FirstOrDefaultAsync(x => x.Id == Id);
                if (data == null)
                {
                    return NotFound();
                }
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int? Id)
        {
            if (HttpContext.Session.GetString("UserSession") == null)

            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();
                return RedirectToAction("Login");
            }
            var data = await context.employees.FindAsync(Id);
            if (data != null)
            {
                context.employees.Remove(data);
            }
            await context.SaveChangesAsync();
            TempData["delete_success"] = "deleted..";

            return RedirectToAction("index", "Admin");

        }


        //EMPLOYEE DASHBOARD WORK


        public IActionResult EmployeeLogin()
        {
            if (HttpContext.Session.GetString("UserEmployeeSession") != null)
            {
                return RedirectToAction("EmployyeeDashboard");

            }
            return View();

        }


        [HttpPost]
        public IActionResult EmployeeLogin(Employee empuser)
        {
            var myempuser = context.employees.Where(x => x.Email == empuser.Email && x.Password == empuser.Password).FirstOrDefault();
            if (myempuser != null)
            {
                HttpContext.Session.SetString("UserEmployeeSession", empuser.Email);
                HttpContext.Session.SetString("NameEmployeeSession", myempuser.Name);
                HttpContext.Session.SetString("ImageEmployeeSession", myempuser.Image_path);


                return RedirectToAction("EmployyeeDashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed..";
            }
            return View();
        }
        public IActionResult EmployyeeDashboard()
        {



            if (HttpContext.Session.GetString("UserEmployeeSession") != null)
            {
                // Fetching employee's name and image path from session
                ViewBag.NameEmployeeSession = HttpContext.Session.GetString("NameEmployeeSession");
                ViewBag.ImageEmployeeSession = HttpContext.Session.GetString("ImageEmployeeSession");

                return View();
            }
            else
            {
                return RedirectToAction("EmployeeLogin");
            }
        }



        public IActionResult EmployeeLogout()
        {
            if (HttpContext.Session.GetString("UserEmployeeSession") != null)
            {
                HttpContext.Session.Remove("UserEmployeeSession");
                HttpContext.Session.Remove("NameEmployeeSession");
                HttpContext.Session.Remove("ImageEmployeeSession");

                return RedirectToAction("EmployeeLogin");

            }
            return View();
        }
        //EMPLOYEE WORK END



        //USER PANEL WORK

        public IActionResult UserDashboard()
        {
            if (HttpContext.Session.GetString("Usession") != null)
            {
                ViewBag.Myusersession = HttpContext.Session.GetString("Usession").ToString();
                ViewBag.Myusernamesession = HttpContext.Session.GetString("Unamesession").ToString();
                ViewBag.Myidsession = HttpContext.Session.GetString("UserId").ToString();


            }
            else
            {
                return RedirectToAction("UserLogin");
            }
            return View();
        }


        public IActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserRegister(User abc)
        {
            if (ModelState.IsValid)
            {
                await context.users.AddAsync(abc);
                await context.SaveChangesAsync();
                TempData["Success"] = "Registered Successfully";
                return RedirectToAction("UserLogin");

            }
            return View();
        }


        public IActionResult UserLogin()
        {
            if (HttpContext.Session.GetString("Usession") != null)
            {
                return RedirectToAction("UserDashboard");

            }
            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(User abc)
        {
            var Myuuser = context.users.Where(x => x.Email == abc.Email && x.Password == abc.Password).FirstOrDefault();
            if (Myuuser != null)
            {
                HttpContext.Session.SetString("Usession", Myuuser.Email);
                HttpContext.Session.SetString("Unamesession", Myuuser.Name);
                HttpContext.Session.SetString("UserId", Myuuser.Id.ToString());


                return RedirectToAction("UserDashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed..";
            }
            return View();
        }


        public IActionResult UserLogout()
        {
            if (HttpContext.Session.GetString("Usession") != null)
            {
                HttpContext.Session.Remove("Usession");
                HttpContext.Session.Remove("Unamesession");
                HttpContext.Session.Remove("UserId");

                return RedirectToAction("UserLogin");

            }
            return View();
        }




        //Admin Panel Insurance  Table Crud Work

        public async Task<IActionResult> Insuranceindex()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {

                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                var Insudata = await context.insurances.ToListAsync();
                return View(Insudata);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }




        public async Task<IActionResult> Addinsurance(Insurance Insudata)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                if (ModelState.IsValid)
                {
                    await context.insurances.AddAsync(Insudata);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Insuranceindex");
                }
                return View(Insudata);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        public async Task<IActionResult> Editinsurance(int? Id)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                if (Id == null || context.insurances == null)
                {
                    return NotFound();
                }
                var Insudata = await context.insurances.FindAsync(Id);
                if (Insudata == null)
                {
                    return NotFound();
                }
                return View(Insudata);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editinsurance(int? Id, Insurance Insudata)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                if (Id != Insudata.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    context.insurances.Update(Insudata);
                    TempData["update_success"] = "updated..";
                    await context.SaveChangesAsync();
                    return RedirectToAction("Insuranceindex", "Admin");
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }






        public async Task<IActionResult> Deleteinsurance(int? Id)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                if (Id == null || context.insurances == null)
                {
                    return NotFound();
                }
                var Insudata = await context.insurances.FirstOrDefaultAsync(x => x.Id == Id);
                if (Insudata == null)
                {
                    return NotFound();
                }
                return View(Insudata);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        [HttpPost, ActionName("Deleteinsurance")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteinsuranceConfirmed(int? Id)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession").ToString();

                var Insudata = await context.insurances.FindAsync(Id);
                if (Insudata != null)
                {
                    context.insurances.Remove(Insudata);
                }
                await context.SaveChangesAsync();
                TempData["delete_success"] = "deleted..";
                return RedirectToAction("Insuranceindex", "Admin");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        public JsonResult IsEmailExists(string email)
        {
            var emailExists = context.admins.Any(a => a.Email == email);
            return Json(emailExists ? "Email Exists" : (object)false);
        }


        public IActionResult InsuranceCards()
        {
            return View();
        }

     
        public IActionResult InsuranceList()
        {
            var userName = HttpContext.Session.GetString("Unamesession");

            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view

                var insurances = context.insurances.ToList(); // Fetch insurances from the database
                return PartialView("InsuranceCards", insurances); // Pass the insurance list to the partial view
            }
            else
            {
                return RedirectToAction("UserLogin"); // Redirect to login if session is empty
            }
        }

   
        public IActionResult ApplyInsurance(int id)
        {
            // Session se username ko hasil karain
            var userName = HttpContext.Session.GetString("Unamesession");

            // Agar user login hai toh process ko jaari rakhein
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view
                // Database se insurance details ko fetch karain jahan id match ho
                var insurance = context.insurances.FirstOrDefault(i => i.Id == id);

                if (insurance == null)
                {
                    return NotFound(); // Agar id match nahi hoti toh error dikhayein
                }

                // Pass InsuranceId to the View using ViewBag
                ViewBag.InsuranceId = id;

                // Insurance data ko ApplyInsurance page par bhejna
                return View(insurance);
            }
            else
            {
                return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
            }
        }


        //Home INSURANCE FORM WORK START
        public IActionResult HomeInsuranceForm()
        {
            // Session se username ko hasil karain
            var userName = HttpContext.Session.GetString("Unamesession");

            // Agar user login hai toh form dikhayein
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HomeInsuranceForm(int Id, HomeViewModel hom) // Added InsuranceId parameter
        {

            string fileName = "";

            // Image Upload Logic
            if (hom.Papersphoto != null)
            {
                var ext = Path.GetExtension(hom.Papersphoto.FileName);
                var size = hom.Papersphoto.Length;

                if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
                {
                    if (size <= 1000000) // 1 MB
                    {
                        string folder = Path.Combine(env.WebRootPath, "Homepaperimage");
                        fileName = Guid.NewGuid().ToString() + "_" + hom.Papersphoto.FileName;
                        string filePath = Path.Combine(folder, fileName);
                        hom.Papersphoto.CopyTo(new FileStream(filePath, FileMode.Create));

                        // Fetch UserId from session
                        var userId = HttpContext.Session.GetString("UserId");

                        if (!string.IsNullOrEmpty(userId))
                        {
                            Home h = new Home()
                            {
                                OwnerName = hom.OwnerName,
                                Address = hom.Address,
                                Type = hom.Type,
                                Stories = hom.Stories,
                                Papers = fileName,
                                Added_by = int.Parse(userId) // Assuming Added_by is an int field
                            };

                            context.homes.Add(h);
                            context.SaveChanges();

                            // Create Insurance Request
                            var insuranceRequest = new InsuranceRequest()
                            {
                                InsuranceId = Id, // Use the passed InsuranceId
                                DetailId = h.Id, // Id of the saved Home entry
                                Added_by = int.Parse(userId), // User Id
                                Remarks = "Requested",
                                Action = "Requested" // Set status to 'Requested'
                            };

                            context.insurancerequests.Add(insuranceRequest); // Assuming your DbSet is named insuranceRequests
                            context.SaveChanges(); // Save changes to the database

                            return RedirectToAction("UserDashboard");
                        }
                        else
                        {
                            TempData["Error"] = "User is not logged in";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Size_Error"] = "Image must be less than 1 MB";
                    }
                }
                else
                {
                    TempData["Ext_Error"] = "Only PNG, JPG, JPEG images allowed";
                }
            }

            return View();
        }


     

        ////Home INSURANCE FORM WORK END

        //LIFE INUSRANCE FORM WORK START
        public IActionResult LifeInsuranceForm()
        {
            // Session se username ko hasil karain
            var userName = HttpContext.Session.GetString("Unamesession");

            // Agar user login hai toh form dikhayein
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LifeInsuranceForm(int Id, Life life) // Added InsuranceId parameter
        {
            // Fetch UserId from session
            var userId = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                Life l = new Life()
                {
                    BankStatement = life.BankStatement, // Assuming you want to take BankStatement from the form
                    CnicNo = life.CnicNo, // CNIC number from the form
                    HealthInsurance = life.HealthInsurance, // Health insurance details from the form
                    Added_by = int.Parse(userId) // User ID
                };

                context.lifes.Add(l);
                context.SaveChanges();

                // Create Insurance Request
                var insuranceRequest = new InsuranceRequest()
                {
                    InsuranceId = Id, // Use the passed InsuranceId
                    DetailId = l.Id, // Id of the saved Life entry
                    Added_by = int.Parse(userId), // User Id
                    Remarks = "Requested",
                    Action = "Requested" // Set status to 'Requested'
                };

                context.insurancerequests.Add(insuranceRequest);
                context.SaveChanges();

                return RedirectToAction("UserDashboard");
            }
            else
            {
                TempData["Error"] = "User is not logged in";
                return View();
            }
        }

        //LIFE INUSRANCE FORM WORK END 

        //MOTOR LIFE INUSRANCE START
        public IActionResult MotorInsuranceForm()
        {
            // Session se username ko hasil karain
            var userName = HttpContext.Session.GetString("Unamesession");

            // Agar user login hai toh form dikhayein
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MotorInsuranceForm(int Id, Motor moto) // Added InsuranceId parameter
        {
            // Fetch UserId from session
            var userId = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                Motor m = new Motor()
                {
                    Model = moto.Model, // Assuming you want to take BankStatement from the form
                    Condition = moto.Condition, // CNIC number from the form
                    VechileNumber = moto.VechileNumber, // Health insurance details from the form
                    Added_by = int.Parse(userId) // User ID
                };

                context.motors.Add(m);
                context.SaveChanges();

                // Create Insurance Request
                var insuranceRequest = new InsuranceRequest()
                {
                    InsuranceId = Id, // Use the passed InsuranceId
                    DetailId = m.Id, // Id of the saved Life entry
                    Added_by = int.Parse(userId), // User Id
                    Remarks = "Requested",
                    Action = "Requested" // Set status to 'Requested'
                };

                context.insurancerequests.Add(insuranceRequest);
                context.SaveChanges();

                return RedirectToAction("UserDashboard");
            }
            else
            {
                TempData["Error"] = "User is not logged in";
                return View();
            }
        }


        //MOTOR LIFE INUSRANCE END

        //HEALTH INUSRANCE FORM WORK
        public IActionResult HealthInsuranceForm()
        {

            // Session se username ko hasil karain
            var userName = HttpContext.Session.GetString("Unamesession");

            // Agar user login hai toh form dikhayein
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HealthInsuranceForm(int Id, HealthViewModel hea) // Added InsuranceId parameter
        {
            string fileName = "";
            if (hea.Imagephoto != null)
            {
                var ext = Path.GetExtension(hea.Imagephoto.FileName);
                var size = hea.Imagephoto.Length;
                if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
                {

                    if (size <= 1000000)//1 MB
                    {

                        string folder = Path.Combine(env.WebRootPath, "healthimage");
                        fileName = Guid.NewGuid().ToString() + "_" + hea.Imagephoto.FileName;
                        string filePath = Path.Combine(folder, fileName);
                        hea.Imagephoto.CopyTo(new FileStream(filePath, FileMode.Create));

                        // Fetch UserId from session
                        var userId = HttpContext.Session.GetString("UserId");

                        if (!string.IsNullOrEmpty(userId))
                        {
                            Health hm = new Health()
                            {
                                Name = hea.Name,
                                CnicNo = hea.CnicNo,
                                HealthDocument = hea.HealthDocument,
                                Image = fileName,
                                Added_by = int.Parse(userId) // Assuming Added_by is an int field

                            };
                            context.healths.Add(hm);
                            context.SaveChanges();

                            // Create Insurance Request
                            var insuranceRequest = new InsuranceRequest()
                            {
                                InsuranceId = Id, // Use the passed InsuranceId
                                DetailId = hm.Id, // Id of the saved Home entry
                                Added_by = int.Parse(userId), // User Id
                                Remarks = "Requested",
                                Action = "Requested" // Set status to 'Requested'
                            };

                            context.insurancerequests.Add(insuranceRequest); // Assuming your DbSet is named insuranceRequests
                            context.SaveChanges(); // Save changes to the database

                            return RedirectToAction("UserDashboard");
                        }
                        else
                        {
                            TempData["Error"] = "User is not logged in";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["Size_Error"] = "Image must be less than 1 MB";
                    }
                }
                else
                {
                    TempData["Ext_Error"] = "Only PNG, JPG, JPEG images allowed";
                }
            }

            return View();
        }
        //HEALTH INUSRANCE FORM END


        //public IActionResult EmployeeInsuranceRequest()
        //{
        //    // Fetching insurance requests with join to Insurance and User table to get InsuranceType and User details
        //    var requests = from ir in context.insurancerequests
        //                   join ins in context.insurances on ir.InsuranceId equals ins.Id
        //                   join usr in context.users on ir.Added_by equals usr.Id // Join with User table to get both ID and Name
        //                   select new InsuranceRequestViewModel
        //                   {
        //                       Id = ir.Id,
        //                       InsuranceId = ir.InsuranceId,
        //                       Title = ins.Type, // Get Insurance Type from Insurance table
        //                       Added_by = ir.Added_by, // Keep Added_by as the ID
        //                       Requested_by = usr.Name, // Fetch User Name from User table
        //                       Status = ir.Action
        //                   };

        //    // Return the list of requests with insurance types, user IDs, and user names to the view
        //    return View(requests.ToList());
        //}

        public IActionResult EmployeeInsuranceRequest()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("UserEmployeeSession") != null)
            {
                // Employee ka naam aur image session se fetch kar rahe hain
                ViewBag.NameEmployeeSession = HttpContext.Session.GetString("NameEmployeeSession");
                ViewBag.ImageEmployeeSession = HttpContext.Session.GetString("ImageEmployeeSession");

                // Insurance requests ko fetch karte hain
                var requests = from ir in context.insurancerequests
                               join ins in context.insurances on ir.InsuranceId equals ins.Id
                               join usr in context.users on ir.Added_by equals usr.Id
                               select new InsuranceRequestViewModel
                               {
                                   Id = ir.Id,
                                   InsuranceId = ir.InsuranceId,
                                   Title = ins.Type,
                                   Added_by = ir.Added_by,
                                   Requested_by = usr.Name,
                                   Status = ir.Action
                               };

                // Insurance requests ko View ke liye pass karte hain
                return View(requests.ToList());
            }
            else
            {
                return RedirectToAction("EmployeeLogin"); // If not logged in, redirect to login page
            }
        }


        public IActionResult Accepted(int Id)
        {
            // InsuranceRequest ko Id se retrieve karein
            var insurancerequest = context.insurancerequests.Find(Id);

            if (insurancerequest == null)
            {
                return NotFound();
            }

            // Action ko update karein
            insurancerequest.Action = "Accepted";

            // Remarks ko "Requested" ka dummy text set karein
            insurancerequest.Remarks = "Requested";

            // Sirf Id, Title (Type), aur Requested_by update karein


            // Database mein changes save karein
            context.SaveChanges();

            // TempData se success message set karein
            TempData["update_success"] = "Insurance request has been accepted.";

            // Redirect karein kisi view ya action pe
            return RedirectToAction("EmployeeInsuranceRequest");
        }


        [HttpPost]
        public IActionResult Rejected(int Id, string remarks)
        {
            // InsuranceRequest ko Id se retrieve karein
            var insurancerequest = context.insurancerequests.Find(Id);

            if (insurancerequest == null)
            {
                return NotFound();
            }

            insurancerequest.Remarks = remarks;
            insurancerequest.Action = "Rejected";

            // Sirf Id, Title (Type), aur Requested_by update karein


            // Database mein changes save karein
            context.SaveChanges();

            // TempData se success message set karein
            TempData["update_danger"] = "Insurance request has been Rejected.";

            // Redirect karein kisi view ya action pe
            return Ok(); // AJAX request ke liye successful response bhejein
        }




        [HttpPost]
        public IActionResult Remarks(int requestId, string remarks)
        {
            var request = context.insurancerequests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Remarks = remarks;
                context.SaveChanges();
                TempData["update_success"] = "Remarks saved successfully!";
            }
            return RedirectToAction("EmployeeInsuranceRequest");
        }

        //public IActionResult EmployeeAcceptedRequest()
        //{
        //    // Fetching only accepted insurance requests
        //    var acceptedRequests = from ir in context.insurancerequests
        //                           join ins in context.insurances on ir.InsuranceId equals ins.Id
        //                           join usr in context.users on ir.Added_by equals usr.Id // Join with User table to get Name
        //                           where ir.Action == "Accepted" // Filter for accepted status
        //                           select new InsuranceRequestViewModel
        //                           {
        //                               Id = ir.Id,
        //                               InsuranceId = ir.InsuranceId,
        //                               Title = ins.Type, // Get Insurance Type from Insurance table
        //                               Added_by = ir.Added_by, // Keep Added_by as the ID
        //                               Requested_by = usr.Name, // Fetch User Name from User table
        //                               Status = ir.Action
        //                           };

        //    // Return only accepted requests to the view
        //    return View(acceptedRequests.ToList());
        //}

        public IActionResult EmployeeAcceptedRequest()
        {
            // Check if the employee is logged in
            if (HttpContext.Session.GetString("UserEmployeeSession") != null)
            {
                // Fetch employee's name and image path from session
                ViewBag.NameEmployeeSession = HttpContext.Session.GetString("NameEmployeeSession");
                ViewBag.ImageEmployeeSession = HttpContext.Session.GetString("ImageEmployeeSession");

                // Fetch only accepted insurance requests
                var acceptedRequests = from ir in context.insurancerequests
                                       join ins in context.insurances on ir.InsuranceId equals ins.Id
                                       join usr in context.users on ir.Added_by equals usr.Id
                                       where ir.Action == "Accepted" // Filter for accepted status
                                       select new InsuranceRequestViewModel
                                       {
                                           Id = ir.Id,
                                           InsuranceId = ir.InsuranceId,
                                           Title = ins.Type,
                                           Added_by = ir.Added_by,
                                           Requested_by = usr.Name,
                                           Status = ir.Action
                                       };

                // Return only accepted requests to the view
                return View(acceptedRequests.ToList());
            }
            else
            {
                // Redirect to login page if not logged in
                return RedirectToAction("EmployeeLogin");
            }
        }

        //public IActionResult EmployeeRejectedRequest()
        //{
        //    // Fetching only rejected insurance requests
        //    var rejectedRequests = from ir in context.insurancerequests
        //                           join ins in context.insurances on ir.InsuranceId equals ins.Id
        //                           join usr in context.users on ir.Added_by equals usr.Id // Join with User table to get Name
        //                           where ir.Action == "Rejected" // Filter for rejected status
        //                           select new InsuranceRequestViewModel
        //                           {
        //                               Id = ir.Id,
        //                               InsuranceId = ir.InsuranceId,
        //                               Title = ins.Type, // Get Insurance Type from Insurance table
        //                               Added_by = ir.Added_by, // Keep Added_by as the ID
        //                               Requested_by = usr.Name, // Fetch User Name from User table
        //                               Status = ir.Action,
        //                               Remarks = ir.Remarks // Get remarks from the insurance request table
        //                           };

        //    // Return only rejected requests with remarks to the view
        //    return View(rejectedRequests.ToList());
        //}

        public IActionResult EmployeeRejectedRequest()
        {
            // Pehle session check karte hain
            if (HttpContext.Session.GetString("UserEmployeeSession") != null)
            {
                // Employee ka naam aur image session se fetch kar rahe hain
                ViewBag.NameEmployeeSession = HttpContext.Session.GetString("NameEmployeeSession");
                ViewBag.ImageEmployeeSession = HttpContext.Session.GetString("ImageEmployeeSession");

                // Sirf rejected insurance requests ko fetch kar rahe hain
                var rejectedRequests = from ir in context.insurancerequests
                                       join ins in context.insurances on ir.InsuranceId equals ins.Id
                                       join usr in context.users on ir.Added_by equals usr.Id
                                       where ir.Action == "Rejected" // Rejected status ke liye filter
                                       select new InsuranceRequestViewModel
                                       {
                                           Id = ir.Id,
                                           InsuranceId = ir.InsuranceId,
                                           Title = ins.Type,
                                           Added_by = ir.Added_by,
                                           Requested_by = usr.Name,
                                           Status = ir.Action,
                                           Remarks = ir.Remarks // Remarks field ko get karte hain
                                       };

                return View(rejectedRequests.ToList()); // View ke liye rejected requests ko pass karte hain
            }
            else
            {
                return RedirectToAction("EmployeeLogin"); // Agar login na ho toh redirect karenge
            }
        }



        public IActionResult PendingInsurance()
        {
            // Check if user is logged in
            var userName = HttpContext.Session.GetString("Unamesession");

            // Agar user login hai toh form dikhayein
            if (!string.IsNullOrEmpty(userName))
            {
                ViewBag.UserName = userName; // Send the username to the view

                // Fetching insurance requests where Status is "Requested"
                var requests = from ir in context.insurancerequests
                               join ins in context.insurances on ir.InsuranceId equals ins.Id
                               join usr in context.users on ir.Added_by equals usr.Id
                               where ir.Action == "Requested" && usr.Name == userName // Filter by Requested status
                               select new InsuranceRequestViewModel
                               {
                                   Id = ir.Id,
                                   InsuranceId = ir.InsuranceId,
                                   Title = ins.Type,
                                   Added_by = ir.Added_by,
                                   Requested_by = usr.Name,
                                   Status = ir.Action
                               };
				// Check if requests list is empty
				if (!requests.Any())
				{
					ViewBag.ErrorMessage = "No insurance requests found.";
				}
				return View(requests.ToList());
            }
            else
            {
                return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
            }
        }


		public IActionResult ActiveInsurance()
		{
			// Check if user is logged in
			var userName = HttpContext.Session.GetString("Unamesession");

			// Agar user login hai toh form dikhayein
			if (!string.IsNullOrEmpty(userName))
			{
				ViewBag.UserName = userName; // Send the username to the view

				// Fetching insurance requests where Status is "Requested"
				var requests = from ir in context.insurancerequests
							   join ins in context.insurances on ir.InsuranceId equals ins.Id
							   join usr in context.users on ir.Added_by equals usr.Id
							   where ir.Action == "Accepted" && usr.Name == userName // Filter by Requested status
							   select new InsuranceRequestViewModel
							   {
								   Id = ir.Id,
								   InsuranceId = ir.InsuranceId,
								   Title = ins.Type,
								   Added_by = ir.Added_by,
								   Requested_by = usr.Name,
								   Status = ir.Action,
                                   Remarks = "Congratulations!"
							   };
				// Check if requests list is empty
				if (!requests.Any())
				{
					ViewBag.ErrorMessage = "No insurance requests found.";
				}
				return View(requests.ToList());
			}
			else
			{
				return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
			}
		}


		public IActionResult RejectedInsurance()
		{
			// Check if user is logged in
			var userName = HttpContext.Session.GetString("Unamesession");

			// Agar user login hai toh form dikhayein
			if (!string.IsNullOrEmpty(userName))
			{
				ViewBag.UserName = userName; // Send the username to the view

				// Fetching insurance requests where Status is "Requested"
				var requests = from ir in context.insurancerequests
							   join ins in context.insurances on ir.InsuranceId equals ins.Id
							   join usr in context.users on ir.Added_by equals usr.Id
							   where ir.Action == "Rejected" && usr.Name == userName // Filter by Requested status
							   select new InsuranceRequestViewModel
							   {
								   Id = ir.Id,
								   InsuranceId = ir.InsuranceId,
								   Title = ins.Type,
								   Added_by = ir.Added_by,
								   Requested_by = usr.Name,
								   Status = ir.Action,
								   Remarks = ir.Remarks
							   };
				// Check if requests list is empty
				if (!requests.Any())
				{
					ViewBag.ErrorMessage = "No insurance requests found.";
				}
				return View(requests.ToList());
			}
			else
			{
				return RedirectToAction("UserLogin"); // Agar session khali hai, toh login page par redirect karein
			}
		}


        //public IActionResult GetInsuranceDetails(int detailId, string insuranceType)
        //{
        //    if (detailId == 0 || string.IsNullOrEmpty(insuranceType))
        //    {
        //        return BadRequest("Invalid parameters.");
        //    }

        //    object details = null;

        //    try
        //    {
        //        switch (insuranceType)
        //        {
        //            case "Home":
        //                details = context.homes.SingleOrDefault(h => h.Id == detailId);
        //                break;
        //            case "Life":
        //                details = context.lifes.SingleOrDefault(l => l.Id == detailId);
        //                break;
        //            case "Health":
        //                details = context.healths.SingleOrDefault(h => h.Id == detailId);
        //                break;
        //            case "Motor":
        //                details = context.motors.SingleOrDefault(m => m.Id == detailId);
        //                break;
        //            default:
        //                return BadRequest("Unknown insurance type.");
        //        }

        //        if (details == null)
        //            return NotFound("Details not found.");

        //        return PartialView("_InsuranceDetailsModal", details);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Server error: " + ex.Message);
        //    }
        //}


    }

}


