using FibergCarRental.Models;
using FibergCarRental.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FibergCarRental.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult Login(int? carId)
        {
            ViewBag.CarId = carId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, int? carId)
        {
            try
            {
                Console.WriteLine($"Login attempt with email: {email}, carId: {carId}");
                var customer = await _customerRepository.GetByEmailAndPasswordAsync(email, password);
                if (customer == null)
                {
                    ViewBag.Error = "Invalid email or password.";
                    ViewBag.CarId = carId;
                    return View();
                }

                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                Console.WriteLine($"Login successful, CustomerId {customer.Id} set in session");
                return carId.HasValue
                    ? RedirectToAction("Book", "Booking", new { carId })
                    : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Login: {ex.Message}");
                ViewBag.Error = "Login failed. Please try again.";
                ViewBag.CarId = carId;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register(int? carId)
        {
            ViewBag.CarId = carId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, int? carId)
        {
            try
            {
                Console.WriteLine($"Register attempt with email: {email}, carId: {carId}");
                var existingCustomer = await _customerRepository.GetByEmailAsync(email);
                if (existingCustomer != null)
                {
                    ViewBag.Error = "This email is already registered. Please use a different email or log in.";
                    ViewBag.CarId = carId;
                    return View();
                }

                var customer = new Customer { FirstName = firstName, LastName = lastName, Email = email, Password = password };
                await _customerRepository.AddAsync(customer);
                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                Console.WriteLine($"Register successful, CustomerId {customer.Id} set in session");
                return carId.HasValue
                    ? RedirectToAction("Book", "Booking", new { carId })
                    : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Register: {ex.Message}");
                ViewBag.Error = "Registration failed. Please try again.";
                ViewBag.CarId = carId;
                return View();
            }
        }
    }
}