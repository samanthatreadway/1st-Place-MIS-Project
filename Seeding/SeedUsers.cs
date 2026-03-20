using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using fa25group23final.Utilities;
using fa25group23final.DAL;
using fa25group23final.Models;

namespace fa25group23final.Seeding
{
    public static class SeedUsers
    {
        public async static Task<IdentityResult> SeedAllUsers(UserManager<AppUser> userManager, AppDbContext context)
        {
            //Create a list of AddUserModels
            List<AddUserModel> AllUsers = new List<AddUserModel>();

        /*    AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    PhoneNumber = "(512)555-1234",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Address = "123 Street, Austin TX, 78705",
                    Status = true
                },
                Password = "Abc123!",
                RoleName = "Admin"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "employee@example.com",
                    Email = "employee@example.com",
                    PhoneNumber = "(512)555-1234",
                    FirstName = "Employee",
                    LastName = "Employee",
                    Address = "123 Street, Austin TX, 78705",
                    Status = true
                },
                Password = "Abc123!",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "bevo@example.com",
                    Email = "bevo@example.com",
                    PhoneNumber = "(512)555-5555",
                    FirstName = "Bevo",
                    LastName = "Hookem",
                    Address = "123 Street, Austin TX, 78705",
                    Status = true
                },
                Password = "Password123!",
                RoleName = "Customer"
            }); */

            // Employees from Excel
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "c.baker@bevosbooks.com",
                    Email = "c.baker@bevosbooks.com",
                    PhoneNumber = "3395325649",
                    FirstName = "Christopher",
                    LastName = "Baker",
                    Address = "1245 Lake Libris Dr., Cedar Park TX 78613",
                    Status = true
                },
                Password = "dewey4",
                RoleName = "Admin"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "s.barnes@bevosbooks.com",
                    Email = "s.barnes@bevosbooks.com",
                    PhoneNumber = "9636389416",
                    FirstName = "Susan",
                    LastName = "Barnes",
                    Address = "888 S. Main, Kyle TX 78640",
                    Status = true
                },
                Password = "smitty",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "h.garcia@bevosbooks.com",
                    Email = "h.garcia@bevosbooks.com",
                    PhoneNumber = "4547135738",
                    FirstName = "Hector",
                    LastName = "Garcia",
                    Address = "777 PBR Drive, Austin TX 78712",
                    Status = true
                },
                Password = "squirrel",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "b.ingram@bevosbooks.com",
                    Email = "b.ingram@bevosbooks.com",
                    PhoneNumber = "5817343315",
                    FirstName = "Brad",
                    LastName = "Ingram",
                    Address = "6548 La Posada Ct., Austin TX 78705",
                    Status = true
                },
                Password = "changalang",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.jackson@bevosbooks.com",
                    Email = "j.jackson@bevosbooks.com",
                    PhoneNumber = "8241915317",
                    FirstName = "Jack",
                    LastName = "Jackson",
                    Address = "222 Main, Austin TX 78760",
                    Status = true
                },
                Password = "rhythm",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "t.jacobs@bevosbooks.com",
                    Email = "t.jacobs@bevosbooks.com",
                    PhoneNumber = "2477822475",
                    FirstName = "Todd",
                    LastName = "Jacobs",
                    Address = "4564 Elm St., Georgetown TX 78628",
                    Status = true
                },
                Password = "approval",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "l.jones@bevosbooks.com",
                    Email = "l.jones@bevosbooks.com",
                    PhoneNumber = "4764966462",
                    FirstName = "Lester",
                    LastName = "Jones",
                    Address = "999 LeBlat, Austin TX 78747",
                    Status = true
                },
                Password = "society",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "b.larson@bevosbooks.com",
                    Email = "b.larson@bevosbooks.com",
                    PhoneNumber = "3355258855",
                    FirstName = "Bill",
                    LastName = "Larson",
                    Address = "1212 N. First Ave, Round Rock TX 78665",
                    Status = true
                },
                Password = "tanman",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "v.lawrence@bevosbooks.com",
                    Email = "v.lawrence@bevosbooks.com",
                    PhoneNumber = "7511273054",
                    FirstName = "Victoria",
                    LastName = "Lawrence",
                    Address = "6639 Bookworm Ln., Austin TX 78712",
                    Status = true
                },
                Password = "longhorns",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "m.lopez@bevosbooks.com",
                    Email = "m.lopez@bevosbooks.com",
                    PhoneNumber = "7477907070",
                    FirstName = "Marshall",
                    LastName = "Lopez",
                    Address = "90 SW North St, Austin TX 78729",
                    Status = true
                },
                Password = "swansong",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.macleod@bevosbooks.com",
                    Email = "j.macleod@bevosbooks.com",
                    PhoneNumber = "2621216845",
                    FirstName = "Jennifer",
                    LastName = "MacLeod",
                    Address = "2504 Far West Blvd., Austin TX 78705",
                    Status = true
                },
                Password = "fungus",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "e.markham@bevosbooks.com",
                    Email = "e.markham@bevosbooks.com",
                    PhoneNumber = "5028075807",
                    FirstName = "Elizabeth",
                    LastName = "Markham",
                    Address = "7861 Chevy Chase, Austin TX 78785",
                    Status = true
                },
                Password = "median",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "g.martinez@bevosbooks.com",
                    Email = "g.martinez@bevosbooks.com",
                    PhoneNumber = "1994708542",
                    FirstName = "Gregory",
                    LastName = "Martinez",
                    Address = "8295 Sunset Blvd., Austin TX 78712",
                    Status = true
                },
                Password = "decorate",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.mason@bevosbooks.com",
                    Email = "j.mason@bevosbooks.com",
                    PhoneNumber = "1748136441",
                    FirstName = "Jack",
                    LastName = "Mason",
                    Address = "444 45th St, Austin TX 78701",
                    Status = true
                },
                Password = "rankmary",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "c.miller@bevosbooks.com",
                    Email = "c.miller@bevosbooks.com",
                    PhoneNumber = "8999319585",
                    FirstName = "Charles",
                    LastName = "Miller",
                    Address = "8962 Main St., Austin TX 78709",
                    Status = true
                },
                Password = "kindly",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "m.nguyen@bevosbooks.com",
                    Email = "m.nguyen@bevosbooks.com",
                    PhoneNumber = "8716746381",
                    FirstName = "Mary",
                    LastName = "Nguyen",
                    Address = "465 N. Bear Cub, Austin TX 78734",
                    Status = true
                },
                Password = "ricearoni",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "s.rankin@bevosbooks.com",
                    Email = "s.rankin@bevosbooks.com",
                    PhoneNumber = "5239029525",
                    FirstName = "Suzie",
                    LastName = "Rankin",
                    Address = "23 Dewey Road, Austin TX 78712",
                    Status = true
                },
                Password = "walkamile",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "m.rhodes@bevosbooks.com",
                    Email = "m.rhodes@bevosbooks.com",
                    PhoneNumber = "1232139514",
                    FirstName = "Megan",
                    LastName = "Rhodes",
                    Address = "4587 Enfield Rd., Austin TX 78729",
                    Status = true
                },
                Password = "ingram45",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "e.rice@bevosbooks.com",
                    Email = "e.rice@bevosbooks.com",
                    PhoneNumber = "2706602803",
                    FirstName = "Eryn",
                    LastName = "Rice",
                    Address = "3405 Rio Grande, Austin TX 78746",
                    Status = true
                },
                Password = "arched",
                RoleName = "Admin"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "a.rogers@bevosbooks.com",
                    Email = "a.rogers@bevosbooks.com",
                    PhoneNumber = "4139645586",
                    FirstName = "Allen",
                    LastName = "Rogers",
                    Address = "4965 Oak Hill, Austin TX 78705",
                    Status = true
                },
                Password = "lottery",
                RoleName = "Admin"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "s.saunders@bevosbooks.com",
                    Email = "s.saunders@bevosbooks.com",
                    PhoneNumber = "9036349587",
                    FirstName = "Sarah",
                    LastName = "Saunders",
                    Address = "332 Avenue C, Austin TX 78733",
                    Status = true
                },
                Password = "nostalgic",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "w.sewell@bevosbooks.com",
                    Email = "w.sewell@bevosbooks.com",
                    PhoneNumber = "7224308314",
                    FirstName = "William",
                    LastName = "Sewell",
                    Address = "2365 51st St., Austin TX 78755",
                    Status = true
                },
                Password = "offbeat",
                RoleName = "Admin"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "m.sheffield@bevosbooks.com",
                    Email = "m.sheffield@bevosbooks.com",
                    PhoneNumber = "9349192978",
                    FirstName = "Martin",
                    LastName = "Sheffield",
                    Address = "3886 Avenue A, San Marcos TX 78666",
                    Status = true
                },
                Password = "evanescent",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "c.silva@bevosbooks.com",
                    Email = "c.silva@bevosbooks.com",
                    PhoneNumber = "4874328170",
                    FirstName = "Cindy",
                    LastName = "Silva",
                    Address = "900 4th St, Austin TX 78758",
                    Status = true
                },
                Password = "stewboy",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "e.stuart@bevosbooks.com",
                    Email = "e.stuart@bevosbooks.com",
                    PhoneNumber = "1967846827",
                    FirstName = "Eric",
                    LastName = "Stuart",
                    Address = "5576 Toro Ring, Austin TX 78758",
                    Status = true
                },
                Password = "instrument",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.tanner@bevosbooks.com",
                    Email = "j.tanner@bevosbooks.com",
                    PhoneNumber = "5923026779",
                    FirstName = "Jeremy",
                    LastName = "Tanner",
                    Address = "4347 Almstead, Austin TX 78712",
                    Status = true
                },
                Password = "hecktour",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "a.taylor@bevosbooks.com",
                    Email = "a.taylor@bevosbooks.com",
                    PhoneNumber = "7246195827",
                    FirstName = "Allison",
                    LastName = "Taylor",
                    Address = "467 Nueces St., Austin TX 78727",
                    Status = true
                },
                Password = "countryrhodes",
                RoleName = "Employee"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "r.taylor@bevosbooks.com",
                    Email = "r.taylor@bevosbooks.com",
                    PhoneNumber = "9071236087",
                    FirstName = "Rachel",
                    LastName = "Taylor",
                    Address = "345 Longview Dr., Austin TX 78746",
                    Status = true
                },
                Password = "landus",
                RoleName = "Admin"
            });

            // Customers from Excel
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cbaker@example.com",
                    Email = "cbaker@example.com",
                    PhoneNumber = "5725458641",
                    FirstName = "Christopher",
                    LastName = "Baker",
                    Address = "1898 Schurz Alley, Austin TX 78705",
                    Status = true
                },
                Password = "bookworm",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "banker@longhorn.net",
                    Email = "banker@longhorn.net",
                    PhoneNumber = "9867048435",
                    FirstName = "Michelle",
                    LastName = "Banks",
                    Address = "97 Elmside Pass, Austin TX 78712",
                    Status = true
                },
                Password = "potato",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "franco@example.com",
                    Email = "franco@example.com",
                    PhoneNumber = "6836109514",
                    FirstName = "Franco",
                    LastName = "Broccolo",
                    Address = "88 Crowley Circle, Austin TX 78786",
                    Status = true
                },
                Password = "painting",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "wchang@example.com",
                    Email = "wchang@example.com",
                    PhoneNumber = "7070911071",
                    FirstName = "Wendy",
                    LastName = "Chang",
                    Address = "56560 Sage Junction, Eagle Pass TX 78852",
                    Status = true
                },
                Password = "texas1",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "limchou@gogle.com",
                    Email = "limchou@gogle.com",
                    PhoneNumber = "1488907687",
                    FirstName = "Lim",
                    LastName = "Chou",
                    Address = "60 Lunder Point, Austin TX 78729",
                    Status = true
                },
                Password = "Anchorage",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "shdixon@aoll.com",
                    Email = "shdixon@aoll.com",
                    PhoneNumber = "6899701824",
                    FirstName = "Shan",
                    LastName = "Dixon",
                    Address = "9448 Pleasure Avenue, Georgetown TX 78628",
                    Status = true
                },
                Password = "aggies",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.b.evans@aheca.org",
                    Email = "j.b.evans@aheca.org",
                    PhoneNumber = "9986825917",
                    FirstName = "Jim Bob",
                    LastName = "Evans",
                    Address = "51 Emmet Parkway, Austin TX 78705",
                    Status = true
                },
                Password = "hampton1",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "feeley@penguin.org",
                    Email = "feeley@penguin.org",
                    PhoneNumber = "3464121966",
                    FirstName = "Lou Ann",
                    LastName = "Feeley",
                    Address = "65 Darwin Crossing, Austin TX 78704",
                    Status = true
                },
                Password = "longhorns",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tfreeley@minnetonka.ci.us",
                    Email = "tfreeley@minnetonka.ci.us",
                    PhoneNumber = "6581357270",
                    FirstName = "Tesa",
                    LastName = "Freeley",
                    Address = "7352 Loftsgordon Court, College Station TX 77840",
                    Status = true
                },
                Password = "mustangs",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "mgarcia@gogle.com",
                    Email = "mgarcia@gogle.com",
                    PhoneNumber = "3767347949",
                    FirstName = "Margaret",
                    LastName = "Garcia",
                    Address = "7 International Road, Austin TX 78756",
                    Status = true
                },
                Password = "onetime",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "chaley@thug.com",
                    Email = "chaley@thug.com",
                    PhoneNumber = "2198604221",
                    FirstName = "Charles",
                    LastName = "Haley",
                    Address = "8 Warrior Trail, Austin TX 78746",
                    Status = true
                },
                Password = "pepperoni",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "jeffh@sonic.com",
                    Email = "jeffh@sonic.com",
                    PhoneNumber = "1222185888",
                    FirstName = "Jeffrey",
                    LastName = "Hampton",
                    Address = "9107 Lighthouse Bay Road, Austin TX 78756",
                    Status = true
                },
                Password = "raiders",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "wjhearniii@umich.org",
                    Email = "wjhearniii@umich.org",
                    PhoneNumber = "5123071976",
                    FirstName = "John",
                    LastName = "Hearn",
                    Address = "59784 Pierstorff Center, Liberty TX 77575",
                    Status = true
                },
                Password = "jhearn22",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ahick@yaho.com",
                    Email = "ahick@yaho.com",
                    PhoneNumber = "1211949601",
                    FirstName = "Anthony",
                    LastName = "Hicks",
                    Address = "932 Monica Way, San Antonio TX 78203",
                    Status = true
                },
                Password = "hickhickup",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ingram@jack.com",
                    Email = "ingram@jack.com",
                    PhoneNumber = "1372121569",
                    FirstName = "Brad",
                    LastName = "Ingram",
                    Address = "4 Lukken Court, New Braunfels TX 78132",
                    Status = true
                },
                Password = "ingram2015",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "toddj@yourmom.com",
                    Email = "toddj@yourmom.com",
                    PhoneNumber = "8543163836",
                    FirstName = "Todd",
                    LastName = "Jacobs",
                    Address = "7 Susan Junction, New York NY 10101",
                    Status = true
                },
                Password = "toddy25",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "thequeen@aska.net",
                    Email = "thequeen@aska.net",
                    PhoneNumber = "3214163359",
                    FirstName = "Victoria",
                    LastName = "Lawrence",
                    Address = "669 Oak Junction, Lockhart TX 78644",
                    Status = true
                },
                Password = "something",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "linebacker@gogle.com",
                    Email = "linebacker@gogle.com",
                    PhoneNumber = "2505265350",
                    FirstName = "Erik",
                    LastName = "Lineback",
                    Address = "099 Luster Point, Kingwood TX 77325",
                    Status = true
                },
                Password = "Password1",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "elowe@netscare.net",
                    Email = "elowe@netscare.net",
                    PhoneNumber = "4070619503",
                    FirstName = "Ernest",
                    LastName = "Lowe",
                    Address = "35473 Hansons Hill, Beverly Hills CA 90210",
                    Status = true
                },
                Password = "aclfest2017",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cluce@gogle.com",
                    Email = "cluce@gogle.com",
                    PhoneNumber = "7358436110",
                    FirstName = "Chuck",
                    LastName = "Luce",
                    Address = "4 Emmet Junction, Navasota TX 77868",
                    Status = true
                },
                Password = "nothinggood",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "mackcloud@george.com",
                    Email = "mackcloud@george.com",
                    PhoneNumber = "7240178229",
                    FirstName = "Jennifer",
                    LastName = "MacLeod",
                    Address = "3 Orin Road, Austin TX 78712",
                    Status = true
                },
                Password = "whatever",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cmartin@beets.com",
                    Email = "cmartin@beets.com",
                    PhoneNumber = "2495200223",
                    FirstName = "Elizabeth",
                    LastName = "Markham",
                    Address = "8171 Commercial Crossing, Austin TX 78712",
                    Status = true
                },
                Password = "snowsnow",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "clarence@yoho.com",
                    Email = "clarence@yoho.com",
                    PhoneNumber = "4086179161",
                    FirstName = "Clarence",
                    LastName = "Martin",
                    Address = "96 Anthes Place, Schenectady NY 12345",
                    Status = true
                },
                Password = "whocares",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "gregmartinez@drdre.com",
                    Email = "gregmartinez@drdre.com",
                    PhoneNumber = "9371927523",
                    FirstName = "Gregory",
                    LastName = "Martinez",
                    Address = "10 Northridge Plaza, Austin TX 78717",
                    Status = true
                },
                Password = "xcellent",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cmiller@bob.com",
                    Email = "cmiller@bob.com",
                    PhoneNumber = "5954063857",
                    FirstName = "Charles",
                    LastName = "Miller",
                    Address = "87683 Schmedeman Circle, Austin TX 78727",
                    Status = true
                },
                Password = "mydogspot",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "knelson@aoll.com",
                    Email = "knelson@aoll.com",
                    PhoneNumber = "8929209512",
                    FirstName = "Kelly",
                    LastName = "Nelson",
                    Address = "3244 Ludington Court, Beaumont TX 77720",
                    Status = true
                },
                Password = "spotmydog",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "joewin@xfactor.com",
                    Email = "joewin@xfactor.com",
                    PhoneNumber = "9226301774",
                    FirstName = "Joe",
                    LastName = "Nguyen",
                    Address = "4780 Talisman Court, San Marcos TX 78667",
                    Status = true
                },
                Password = "joejoejoe",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "orielly@foxnews.cnn",
                    Email = "orielly@foxnews.cnn",
                    PhoneNumber = "2537646912",
                    FirstName = "Bill",
                    LastName = "O'Reilly",
                    Address = "4154 Delladonna Plaza, Bergheim TX 78004",
                    Status = true
                },
                Password = "billyboy",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ankaisrad@gogle.com",
                    Email = "ankaisrad@gogle.com",
                    PhoneNumber = "2182889379",
                    FirstName = "Anka",
                    LastName = "Radkovich",
                    Address = "72361 Bayside Drive, Austin TX 78789",
                    Status = true
                },
                Password = "radgirl",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "megrhodes@freserve.co.uk",
                    Email = "megrhodes@freserve.co.uk",
                    PhoneNumber = "9532396075",
                    FirstName = "Megan",
                    LastName = "Rhodes",
                    Address = "76875 Hoffman Point, Orlando FL 32830",
                    Status = true
                },
                Password = "meganr34",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "erynrice@aoll.com",
                    Email = "erynrice@aoll.com",
                    PhoneNumber = "7303815953",
                    FirstName = "Eryn",
                    LastName = "Rice",
                    Address = "048 Elmside Park, South Padre Island TX 78597",
                    Status = true
                },
                Password = "ricearoni",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "jorge@noclue.com",
                    Email = "jorge@noclue.com",
                    PhoneNumber = "3677322422",
                    FirstName = "Jorge",
                    LastName = "Rodriguez",
                    Address = "01 Browning Pass, Austin TX 78744",
                    Status = true
                },
                Password = "alaskaboy",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "mrrogers@lovelyday.com",
                    Email = "mrrogers@lovelyday.com",
                    PhoneNumber = "3911705385",
                    FirstName = "Allen",
                    LastName = "Rogers",
                    Address = "844 Anderson Alley, Canyon Lake TX 78133",
                    Status = true
                },
                Password = "bunnyhop",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "stjean@athome.com",
                    Email = "stjean@athome.com",
                    PhoneNumber = "7351610920",
                    FirstName = "Olivier",
                    LastName = "Saint-Jean",
                    Address = "1891 Docker Point, Austin TX 78779",
                    Status = true
                },
                Password = "dustydusty",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "saunders@pen.com",
                    Email = "saunders@pen.com",
                    PhoneNumber = "5269661692",
                    FirstName = "Sarah",
                    LastName = "Saunders",
                    Address = "1469 Upham Road, Austin TX 78720",
                    Status = true
                },
                Password = "jrod2017",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "willsheff@email.com",
                    Email = "willsheff@email.com",
                    PhoneNumber = "1875727246",
                    FirstName = "William",
                    LastName = "Sewell",
                    Address = "1672 Oak Valley Circle, Austin TX 78705",
                    Status = true
                },
                Password = "martin1234",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "sheffiled@gogle.com",
                    Email = "sheffiled@gogle.com",
                    PhoneNumber = "1394323615",
                    FirstName = "Martin",
                    LastName = "Sheffield",
                    Address = "816 Kennedy Place, Round Rock TX 78680",
                    Status = true
                },
                Password = "penguin12",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "johnsmith187@aoll.com",
                    Email = "johnsmith187@aoll.com",
                    PhoneNumber = "6645937874",
                    FirstName = "John",
                    LastName = "Smith",
                    Address = "0745 Golf Road, Austin TX 78760",
                    Status = true
                },
                Password = "rogerthat",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "dustroud@mail.com",
                    Email = "dustroud@mail.com",
                    PhoneNumber = "6470254680",
                    FirstName = "Dustin",
                    LastName = "Stroud",
                    Address = "505 Dexter Plaza, Sweet Home TX 77987",
                    Status = true
                },
                Password = "smitty444",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "estuart@anchor.net",
                    Email = "estuart@anchor.net",
                    PhoneNumber = "7701621022",
                    FirstName = "Eric",
                    LastName = "Stuart",
                    Address = "585 Claremont Drive, Corpus Christi TX 78412",
                    Status = true
                },
                Password = "stewball",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "peterstump@noclue.com",
                    Email = "peterstump@noclue.com",
                    PhoneNumber = "2181960061",
                    FirstName = "Peter",
                    LastName = "Stump",
                    Address = "89035 Welch Circle, Pflugerville TX 78660",
                    Status = true
                },
                Password = "slowwind",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "jtanner@mustang.net",
                    Email = "jtanner@mustang.net",
                    PhoneNumber = "9908469499",
                    FirstName = "Jeremy",
                    LastName = "Tanner",
                    Address = "4 Stang Trail, Austin TX 78702",
                    Status = true
                },
                Password = "tanner5454",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "taylordjay@aoll.com",
                    Email = "taylordjay@aoll.com",
                    PhoneNumber = "7011918647",
                    FirstName = "Allison",
                    LastName = "Taylor",
                    Address = "726 Twin Pines Avenue, Austin TX 78713",
                    Status = true
                },
                Password = "allyrally",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "rtaylor@gogle.com",
                    Email = "rtaylor@gogle.com",
                    PhoneNumber = "8937910053",
                    FirstName = "Rachel",
                    LastName = "Taylor",
                    Address = "06605 Sugar Drive, Austin TX 78712",
                    Status = true
                },
                Password = "taylorbaylor",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "teefrank@noclue.com",
                    Email = "teefrank@noclue.com",
                    PhoneNumber = "6394568913",
                    FirstName = "Frank",
                    LastName = "Tee",
                    Address = "3567 Dawn Plaza, Austin TX 78786",
                    Status = true
                },
                Password = "teeoff22",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ctucker@alphabet.co.uk",
                    Email = "ctucker@alphabet.co.uk",
                    PhoneNumber = "2676838676",
                    FirstName = "Clent",
                    LastName = "Tucker",
                    Address = "704 Northland Alley, San Antonio TX 78279",
                    Status = true
                },
                Password = "tucksack1",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "avelasco@yoho.com",
                    Email = "avelasco@yoho.com",
                    PhoneNumber = "3452909754",
                    FirstName = "Allen",
                    LastName = "Velasco",
                    Address = "72 Harbort Point, Navasota TX 77868",
                    Status = true
                },
                Password = "meow88",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "vinovino@grapes.com",
                    Email = "vinovino@grapes.com",
                    PhoneNumber = "8567089194",
                    FirstName = "Janet",
                    LastName = "Vino",
                    Address = "1 Oak Valley Place, Boston MA 2114",
                    Status = true
                },
                Password = "vinovino",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "westj@pioneer.net",
                    Email = "westj@pioneer.net",
                    PhoneNumber = "6260784394",
                    FirstName = "Jake",
                    LastName = "West",
                    Address = "48743 Banding Parkway, Marble Falls TX 78654",
                    Status = true
                },
                Password = "gowest",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "winner@hootmail.com",
                    Email = "winner@hootmail.com",
                    PhoneNumber = "3733971174",
                    FirstName = "Louis",
                    LastName = "Winthorpe",
                    Address = "96850 Summit Crossing, Austin TX 78730",
                    Status = true
                },
                Password = "louielouie",
                RoleName = "Customer"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "rwood@voyager.net",
                    Email = "rwood@voyager.net",
                    PhoneNumber = "8433359800",
                    FirstName = "Reagan",
                    LastName = "Wood",
                    Address = "18354 Bluejay Street, Austin TX 78712",
                    Status = true
                },
                Password = "woodyman1",
                RoleName = "Customer"
            });

            //create flag to help with errors
            String errorFlag = "Start";

            //create an identity result
            IdentityResult result = IdentityResult.Success;
            try
            {
                foreach (AddUserModel aum in AllUsers)
                {
                    errorFlag = aum.User.Email;
                    // Only create the user if it does not already exist
                    AppUser existingUser = await userManager.FindByEmailAsync(aum.User.Email);
                    if (existingUser == null)
                    {
                        result = await Utilities.AddUser.AddUserWithRoleAsync(aum, userManager, context);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem adding the user with email: " + errorFlag, ex);
            }

            return result;
        }
    }
}