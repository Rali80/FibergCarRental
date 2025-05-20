using FibergCarRental.Models;
using FibergCarRental.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FibergCarRental.Controllers
{
    public class BookingController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingController(ICarRepository carRepository, ICustomerRepository customerRepository, IBookingRepository bookingRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<IActionResult> Book(int carId)
        {
            try
            {
                Console.WriteLine($"Accessing Book with carId: {carId}");
                var car = await _carRepository.GetByIdAsync(carId);
                if (car == null)
                {
                    Console.WriteLine($"Car with ID {carId} not found");
                    return NotFound();
                }
                ViewBag.Car = car;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Book GET: {ex.Message}");
                return View("Error", new { message = $"Failed to load booking page: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Book(int carId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                Console.WriteLine($"Booking POST with carId: {carId}, CustomerId: {customerId}");
                if (!customerId.HasValue)
                {
                    Console.WriteLine("No CustomerId in session, redirecting to Book");
                    return RedirectToAction("Book", new { carId });
                }

                var booking = new Booking
                {
                    CarId = carId,
                    CustomerId = customerId.Value,
                    StartDate = startDate,
                    EndDate = endDate,
                    BookingDate = DateTime.Now
                };

                await _bookingRepository.AddAsync(booking);
                Console.WriteLine($"Booking {booking.Id} created successfully");
                return RedirectToAction("Confirm", new { bookingId = booking.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Book POST: {ex.Message}");
                return View("Error", new { message = $"Failed to create booking: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, int carId)
        {
            try
            {
                Console.WriteLine($"Login attempt with email: {email}, carId: {carId}");
                var customer = await _customerRepository.GetByEmailAndPasswordAsync(email, password);
                if (customer == null)
                {
                    Console.WriteLine("Invalid email or password");
                    ViewBag.Error = "Invalid email or password.";
                    var car = await _carRepository.GetByIdAsync(carId);
                    ViewBag.Car = car ?? new Car { Id = carId }; // Fallback si car es null
                    return View("Book");
                }

                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                Console.WriteLine($"Login successful, CustomerId {customer.Id} set in session");
                return RedirectToAction("Book", new { carId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Login: {ex.Message}");
                ViewBag.Error = $"Login failed: {ex.Message}";
                var car = await _carRepository.GetByIdAsync(carId);
                ViewBag.Car = car ?? new Car { Id = carId };
                return View("Book");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, int carId)
        {
            try
            {
                Console.WriteLine($"Register attempt with email: {email}, carId: {carId}");
                var existingCustomer = await _customerRepository.GetByEmailAndPasswordAsync(email, null);
                if (existingCustomer != null)
                {
                    Console.WriteLine("Email already registered");
                    ViewBag.Error = "This email is already registered. Please use a different email or log in.";
                    var car = await _carRepository.GetByIdAsync(carId);
                    ViewBag.Car = car ?? new Car { Id = carId };
                    return View("Book");
                }

                var customer = new Customer { FirstName = firstName, LastName = lastName, Email = email, Password = password };
                await _customerRepository.AddAsync(customer);
                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                Console.WriteLine($"Register successful, CustomerId {customer.Id} set in session");
                return RedirectToAction("Book", new { carId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Register: {ex.Message}");
                ViewBag.Error = $"Registration failed: {ex.Message}";
                var car = await _carRepository.GetByIdAsync(carId);
                ViewBag.Car = car ?? new Car { Id = carId };
                return View("Book");
            }
        }

        public async Task<IActionResult> Confirm(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    Console.WriteLine($"Booking {bookingId} not found");
                    return NotFound();
                }
                return View(booking);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Confirm: {ex.Message}");
                return View("Error", new { message = $"Failed to load confirmation: {ex.Message}" });
            }
        }

        public async Task<IActionResult> MyBookings()
        {
            try
            {
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                Console.WriteLine($"Accessing MyBookings for CustomerId: {customerId}");
                if (!customerId.HasValue)
                {
                    Console.WriteLine("No CustomerId in session, redirecting to Book");
                    return RedirectToAction("Book", new { carId = 1 });
                }

                var bookings = await _bookingRepository.GetByCustomerIdAsync(customerId.Value);
                Console.WriteLine($"Found {bookings.Count} bookings for CustomerId {customerId}");
                return View(bookings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MyBookings: {ex.Message}");
                return View("Error", new { message = $"Failed to load bookings: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    Console.WriteLine($"Booking {bookingId} not found");
                    return NotFound();
                }
                if (booking.StartDate <= DateTime.Now)
                {
                    Console.WriteLine($"Cannot cancel booking {bookingId}: already started");
                    return BadRequest("Cannot cancel a booking that has already started.");
                }

                await _bookingRepository.DeleteAsync(bookingId);
                Console.WriteLine($"Booking {bookingId} cancelled successfully");
                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CancelBooking: {ex.Message}");
                return View("Error", new { message = $"Failed to cancel booking: {ex.Message}" });
            }
        }

        public IActionResult Logout()
        {
            try
            {
                Console.WriteLine("Logging out, clearing session");
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Logout: {ex.Message}");
                return View("Error", new { message = $"Failed to log out: {ex.Message}" });
            }
        }
    }
}