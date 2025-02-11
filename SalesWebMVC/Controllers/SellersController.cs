﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services;
using System.Diagnostics;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }
        public async Task<IActionResult> Index()
        {
            var sellers = await _sellerService.ListAllAsync();
            return View(sellers);
        }
        public async Task<IActionResult> Details(int id)
        {
            var seller = await _sellerService.GetByIdAsync(id);

            if (seller == null)
            {
                return RedirectToAction(nameof(Error), new { message = "ID not found." });
            }

            return View(seller);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var seller = await _sellerService.GetByIdAsync(id);

            if (seller is null)
            {
                return RedirectToAction(nameof(Error), new { message = "ID not found." });
            }

            var departments = await _departmentService.ListAllAsync();

            SellerFormViewModel viewModel = new() { Seller = seller, Departments = departments };

            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name", seller.DepartmentId);

            return View(viewModel);

        }
        public async Task<IActionResult> Delete(int id)
        {
            var seller = await _sellerService.GetByIdAsync(id);

            if (seller == null)
            {
                return RedirectToAction(nameof(Error), new { message = "ID not found." });
            }

            return View(seller);
        }
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.ListAllAsync();

            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "ID missmatch." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _sellerService.UpdateAsync(seller);
                    return RedirectToAction(nameof(Details), id);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    return RedirectToAction(nameof(Error), new { message = $"Error: {e.Message}" });
                }
            }

            return RedirectToAction(nameof(Edit), id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Seller seller)
        {
            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "ID missmatch." });
            }

            try
            {
                await _sellerService.DeleteAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = $"Error: {e.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _sellerService.CreateAsync(seller);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException e)
                {
                    return RedirectToAction(nameof(Error), new { message = $"Error: {e.Message}" });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}