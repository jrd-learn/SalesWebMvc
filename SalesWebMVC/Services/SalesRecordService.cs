﻿using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Data;
using SalesWebMVC.Models;

namespace SalesWebMVC.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMVCContext _context;

        public SalesRecordService(SalesWebMVCContext context)
        {
            _context = context;
        }

        public DateTime FindMinDate()
        {
            var result = (from obj in _context.SalesRecords select obj)
                .OrderBy(x => x.Date)
                .FirstOrDefault();

            if (result == null)
            {
                return new DateTime(DateTime.Now.Year, 1, 1);
            }

            return result.Date;
        }

        public DateTime FindMaxDate()
        {
            var result = (from obj in _context.SalesRecords select obj)
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            if (result == null)
            {
                return DateTime.Now;
            }

            return result.Date;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime minDate, DateTime maxDate)
        {
            var result = from obj in _context.SalesRecords select obj;

            return await result.Where(x => x.Date >= minDate && x.Date <= maxDate)
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IGrouping<Department?, SalesRecord>>> FindByDateGroupingAsync(DateTime minDate, DateTime maxDate)
        {
            var result = from obj in _context.SalesRecords select obj;

            return await result.Where(x => x.Date >= minDate && x.Date <= maxDate)
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Seller.Department)
                .ToListAsync();
        }
    }
}
