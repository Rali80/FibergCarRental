using FibergCarRental.Models;
using FibergCarRental.Repository;
using Microsoft.AspNetCore.Mvc;

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

        // Login page
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var admin = await _adminRepository.GetByEmailAndPasswordAsync(email, password);
            if (admin == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            HttpContext.Session.SetInt32("AdminId", admin.Id);
            return RedirectToAction("Index");
        }

        // Admin dashboard
        public IActionResult Index()
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            return View();
        }

        // Manage cars
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

            car.ImageUrls = imageUrls?.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            await _carRepository.AddAsync(car);
            return RedirectToAction("ManageCars");
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

            car.ImageUrls = imageUrls?.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            await _carRepository.UpdateAsync(car);
            return RedirectToAction("ManageCars");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            await _carRepository.DeleteAsync(id);
            return RedirectToAction("ManageCars");
        }

        // Manage customers
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

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            await _customerRepository.AddAsync(customer);
            return RedirectToAction("ManageCustomers");
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

            await _customerRepository.UpdateAsync(customer);
            return RedirectToAction("ManageCustomers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (!HttpContext.Session.GetInt32("AdminId").HasValue)
                return RedirectToAction("Login");

            await _customerRepository.DeleteAsync(id);
            return RedirectToAction("ManageCustomers");
        }

        // Manage bookings
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

            await _bookingRepository.DeleteAsync(id);
            return RedirectToAction("ManageBookings");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Login");
        }
    }
}
