using FibergCarRental.Models;
using FibergCarRental.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FibergCarRental.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IAdminRepository _adminRepository;

        public AdminController(ICarRepository carRepository, ICustomerRepository customerRepository, IBookingRepository bookingRepository, IAdminRepository adminRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _adminRepository = adminRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                Console.WriteLine($"Email ingresado: '{email}'");
                Console.WriteLine($"Password ingresado: '{password}'");

                var admin = await _adminRepository.GetByEmailAndPasswordAsync(email, password);
                if (admin == null)
                {
                    ViewBag.Error = "Invalid credentials.";
                    return View();
                }

                HttpContext.Session.SetInt32("AdminId", admin.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Admin Login: {ex.Message}");
                ViewBag.Error = "Login failed. Please try again.";
                return View();
            }
        }


        public IActionResult Index()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            return View();
        }

        public async Task<IActionResult> ManageCars()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            var cars = await _carRepository.GetAllAsync();
            return View(cars);
        }

        [HttpGet]
        public IActionResult AddCar()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCar(Car car, string imageUrls)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                return View(car);
            }

            try
            {
                car.ImageUrls = imageUrls?.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                await _carRepository.AddAsync(car);
                return RedirectToAction("ManageCars");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddCar: {ex.Message}");
                ViewBag.Error = "Failed to add car. Please try again.";
                return View(car);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            var car = await _carRepository.GetByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> EditCar(Car car, string imageUrls)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                return View(car);
            }

            try
            {
                car.ImageUrls = imageUrls?.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                await _carRepository.UpdateAsync(car);
                return RedirectToAction("ManageCars");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditCar: {ex.Message}");
                ViewBag.Error = "Failed to update car. Please try again.";
                return View(car);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            try
            {
                await _carRepository.DeleteAsync(id);
                return RedirectToAction("ManageCars");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteCar: {ex.Message}");
                return View("Error", new { message = $"Failed to delete car: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ManageCustomers()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            var customers = await _customerRepository.GetAllAsync();
            return View(customers);
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            return View("ManageCustomers");
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                return View(customer);
            }

            try
            {
                await _customerRepository.AddAsync(customer);
                return RedirectToAction("ManageCustomers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddCustomer: {ex.Message}");
                ViewBag.Error = "Failed to add customer. Please try again.";
                return View(customer);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer customer)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                return View(customer);
            }

            try
            {
                await _customerRepository.UpdateAsync(customer);
                return RedirectToAction("ManageCustomers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditCustomer: {ex.Message}");
                ViewBag.Error = "Failed to update customer. Please try again.";
                return View(customer);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            try
            {
                await _customerRepository.DeleteAsync(id);
                return RedirectToAction("ManageCustomers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteCustomer: {ex.Message}");
                return View("Error", new { message = $"Failed to delete customer: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ManageBookings()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            var bookings = await _bookingRepository.GetAllAsync();
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            try
            {
                await _bookingRepository.DeleteAsync(id);
                return RedirectToAction("ManageBookings");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteBooking: {ex.Message}");
                return View("Error", new { message = $"Failed to delete booking: {ex.Message}" });
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Login");
        }
    }
}