using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ThanhCuongShop.Models;
using System.Collections.Generic;

namespace ThanhCuongShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Lấy danh sách sản phẩm từ ProductController
        var products = ThanhCuongShop.Controllers.ProductController.products;
        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
