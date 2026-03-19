using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using fa25group23final.DAL;
using fa25group23final.Models;
using fa25group23final.Utilities;
using System;

namespace fa25group23final.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly PasswordValidator<AppUser> _passwordValidator;
        private readonly AppDbContext _context;

        public AccountController(AppDbContext appDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signIn)
        {
            _context = appDbContext;
            _userManager = userManager;
            _signInManager = signIn;
            //user manager only has one password validator
            _passwordValidator = (PasswordValidator<AppUser>)userManager.PasswordValidators.FirstOrDefault();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel rvm)
        {
            //if registration data is valid, create a new user on the database
            if (ModelState.IsValid == false)
            {
                //this is the sad path - something went wrong, 
                //return the user to the register page to try again
                return View(rvm);
            }

            //this code maps the RegisterViewModel to the AppUser domain model
            AppUser newUser = new AppUser
            {
                UserName = rvm.Email,
                Email = rvm.Email,
                PhoneNumber = rvm.PhoneNumber,

                //Add the rest of the custom user fields here
                //FirstName is included as an example
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                Address = rvm.Address,
                Status = rvm.Status

            };

            //create AddUserModel
            AddUserModel aum = new AddUserModel()
            {
                User = newUser,
                Password = rvm.Password,

                //You will need to change this value if you want to 
                //add the user to a different role - just specify the role name.
                RoleName = "Customer"
            };

            //This code uses the AddUser utility to create a new user with the specified password
            IdentityResult result = await Utilities.AddUser.AddUserWithRoleAsync(aum, _userManager, _context);

            if (result.Succeeded) //everything is okay
            {
                //NOTE: This code logs the user into the account that they just created
                //You may or may not want to log a user in directly after they register - check
                //the business rules!
                //Microsoft.AspNetCore.Identity.SignInResult result2 = await _signInManager.PasswordSignInAsync(rvm.Email, rvm.Password, false, lockoutOnFailure: false);

                //Send the user to the home page
                return RedirectToAction("Index", "Home");
            }
            else  //the add user operation didn't work, and we need to show an error message
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //send user back to page with errors
                return View(rvm);
            }
        }

        // GET: /Account/RegisterEmployee
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterEmployee()
        {
            return View();
        }

        // POST: /Account/RegisterEmployee
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterEmployee(RegisterViewModel rvm)
        {
            //if registration data is valid, create a new user on the database
            if (ModelState.IsValid == false)
            {
                //this is the sad path - something went wrong, 
                //return the user to the register page to try again
                return View(rvm);
            }

            //this code maps the RegisterViewModel to the AppUser domain model
            AppUser newUser = new AppUser
            {
                UserName = rvm.Email,
                Email = rvm.Email,
                PhoneNumber = rvm.PhoneNumber,

                //Add the rest of the custom user fields here
                //FirstName is included as an example
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                Address = rvm.Address,
                Status = rvm.Status

            };

            //create AddUserModel
            AddUserModel aum = new AddUserModel()
            {
                User = newUser,
                Password = rvm.Password,

                //You will need to change this value if you want to 
                //add the user to a different role - just specify the role name.
                RoleName = "Employee"
            };

            //This code uses the AddUser utility to create a new user with the specified password
            IdentityResult result = await Utilities.AddUser.AddUserWithRoleAsync(aum, _userManager, _context);

            if (result.Succeeded) //everything is okay
            {
                //NOTE: This code logs the user into the account that they just created
                //You may or may not want to log a user in directly after they register - check
                //the business rules!
                Microsoft.AspNetCore.Identity.SignInResult result2 = await _signInManager.PasswordSignInAsync(rvm.Email, rvm.Password, false, lockoutOnFailure: false);

                //Send the user to the home page
                return RedirectToAction("Index", "Home");
            }
            else  //the add user operation didn't work, and we need to show an error message
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //send user back to page with errors
                return View(rvm);
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) //user has been redirected here from a page they're not authorized to see
            {
                return View("Error", new string[] { "Access Denied" });
            }
            _signInManager.SignOutAsync(); //this removes any old cookies hanging around
            ViewBag.ReturnUrl = returnUrl; //pass along the page the user should go back to
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel lvm, string? returnUrl)
        {
            //if user forgot to include user name or password,
            //send them back to the login page to try again
            if (ModelState.IsValid == false)
            {
                return View(lvm);
            }

            //attempt to sign the user in using the SignInManager
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(lvm.Email, lvm.Password, lvm.RememberMe, lockoutOnFailure: false);

            //if the login worked, take the user to either the url
            //they requested OR the homepage if there isn't a specific url
            if (result.Succeeded)
            {
                //return ?? "/" means if returnUrl is null, substitute "/" (home)
                return Redirect(returnUrl ?? "/");
            }
            else //log in was not successful
            {
                //add an error to the model to show invalid attempt
                ModelState.AddModelError("", "Invalid login attempt.");
                //send user back to login page to try again
                return View(lvm);
            }
        }

        public IActionResult AccessDenied()
        {
            return View("Error", new string[] { "You are not authorized for this resource" });
        }

        //GET: Account/Index
        public IActionResult Index()
        {
            IndexViewModel ivm = new IndexViewModel();

            //get user info
            String id = User.Identity.Name;
            AppUser user = _context.Users.FirstOrDefault(u => u.UserName == id);

            //populate the view model
            //(i.e. map the domain model to the view model)
            ivm.Email = user.Email;
            ivm.HasPassword = true;
            ivm.UserID = user.Id;
            ivm.UserName = user.UserName;

            //send data to the view
            return View(ivm);
        }



        //Logic for change password
        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }



        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel cpvm)
        {
            //if user forgot a field, send them back to 
            //change password page to try again
            if (ModelState.IsValid == false)
            {
                return View(cpvm);
            }

            //Find the logged in user using the UserManager
            AppUser userLoggedIn = await _userManager.FindByNameAsync(User.Identity.Name);

            //Attempt to change the password using the UserManager
            var result = await _userManager.ChangePasswordAsync(userLoggedIn, cpvm.OldPassword, cpvm.NewPassword);

            //if the attempt to change the password worked
            if (result.Succeeded)
            {
                //sign in the user with the new password
                await _signInManager.SignInAsync(userLoggedIn, isPersistent: false);

                //send the user back to the home page
                return RedirectToAction("Index", "Home");
            }
            else //attempt to change the password didn't work
            {
                //Add all the errors from the result to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //send the user back to the change password page to try again
                return View(cpvm);
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            //sign the user out of the application
            _signInManager.SignOutAsync();

            //send the user back to the home page
            return RedirectToAction("Index", "Home");
        }


        //GET Edit User Page
        public async Task<IActionResult> Edit(string? id)
        {
            //First check if there is id present
            if (id != null) //Means they are trying to update user that is not themselves
            {
                //must check if the user is an admin
                if (!User.IsInRole("Admin"))
                {
                    return View("Error", new string[] { "You do not have permission to edit this account" });
                }
            }


            if (User.IsInRole("Admin")) //Check if User is Admin
            {
                if ( (id != null) && (_userManager.GetUserId(User) != id)) //If Admin and the ID is not Null (checks if trying to edit self)
                {
                    //Check if id is in role Employee, otherwise return error
                    var otherUser = await _userManager.FindByIdAsync(id);

                    if (otherUser == null) //If otherUser is null return error
                    {
                        return View("Error", new string[] { "Other User is Null" });
                    }

                    //Find otherUser role
                    var otherRole = await _userManager.GetRolesAsync(otherUser);

                    if (!otherRole.Contains("Employee")) //Validates other user is Employee
                    {
                        return View("Error", new string[] { "User information you are trying to edit is not an Employee!" });
                    }
                }
            }


            AppUser user;

            if (id == null) //IF ID Null, then the account edited is self
            {
                //Get Current Logged In User's ID
                var userID = _userManager.GetUserId(User);
                //Create AppUser object of logged in User using userID
                user = await _userManager.FindByIdAsync(userID);

            }

            else // else the account is someone else, i.e. admin edditing employee
            {
                var userID = id;
                //Create AppUser object of logged in User using userID
                user = await _userManager.FindByIdAsync(userID);
            }


            //Validate user is not null
            if (user == null)
            {
                return NotFound();
            }

            //Create new ViewModelObject to pass into the Edit View
            var EditAccountObject = new EditAccount
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                ID = user.Id
            };

            return View(EditAccountObject);
            
        }

        //POST Edit Account information
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(EditAccount editAccount)
        {
            //validate model state
            if (ModelState.IsValid == false)
            {
                return View("Edit", editAccount);
            }

            AppUser user;

            //FIND USER ID WHO WE ARE EDITTING
            //admin editing another user
            if (!string.IsNullOrEmpty(editAccount.ID) && User.IsInRole("Admin"))
            {
                user = await _userManager.FindByIdAsync(editAccount.ID);
            }

            //employee editing themselves
            else if (!string.IsNullOrEmpty(editAccount.ID) && User.IsInRole("Employee"))
            {
                user = await _userManager.FindByIdAsync(editAccount.ID);
            }

            //customer editing themselves
            else
            {
                user = await _userManager.GetUserAsync(User);
            }

            //validate user exists
            if (user == null)
            {
                return NotFound();
            }

            //UPDATE THE PROPERTIES

            if (User.IsInRole("Admin"))
            {
                //Logged In User is Admin and is Editing other Account (employee)
                //user.FirstName = editAccount.FirstName;
                //user.LastName = editAccount.LastName;
                user.Address = editAccount.Address;
                user.PhoneNumber = editAccount.PhoneNumber;
            }
            else if (User.IsInRole("Employee"))
            {
                //Logged in User is employee and can only change certain fields
                user.Address = editAccount.Address;
                user.PhoneNumber = editAccount.PhoneNumber;
            }
            else
            {
                //User is Customer and is changing everything else
                user.FirstName = editAccount.FirstName;
                user.LastName = editAccount.LastName;
                user.Address = editAccount.Address;
                user.PhoneNumber = editAccount.PhoneNumber;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return View("Error", new string[] { "Failed to Update the User" });
            }

            return RedirectToAction("Index", "Home");

        }


        //GET: Details View of Account
        public async Task<IActionResult> Details()
        {

            //Get Current Logged In User's ID
            var userID = _userManager.GetUserId(User);
            //Create AppUser object of logged in User using userID
            var user = await _userManager.FindByIdAsync(userID);

            //Create ViewModel object of EditAccount --
            //Reusing this viewmodel because it has all the variables,
            //except were not posting to eidt, just viewing details
            var EditAccountObject = new EditAccount
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                ID = user.Id
            };

            //Send in EditViewModel Object to the View
            return View(EditAccountObject);

        }

    }
}