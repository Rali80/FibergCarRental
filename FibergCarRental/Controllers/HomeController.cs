using FibergCarRental.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibergCarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarRepository _carRepository;

        public HomeController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllAsync();
            return View(cars);
        }

        public async Task<IActionResult> CarDetails(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }
    }
}
