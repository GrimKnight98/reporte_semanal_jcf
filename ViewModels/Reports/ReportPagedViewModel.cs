using System;
using System.Collections.Generic;
using DemoMvc.Models;

namespace DemoMvc.ViewModels.Reports
{
    public class ReportPagedViewModel
    {
        // 🔹 Datos
        public List<Report> Items { get; set; } = new();

        // 🔹 Paginación
        public int CurrentPage { get; set; } = 1;

        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 5 : value > 50 ? 50 : value;
        }

        public int TotalItems { get; set; }

        public int TotalPages => TotalItems == 0 
            ? 1 
            : (int)Math.Ceiling((double)TotalItems / PageSize);

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        // 🔹 Filtros
        public ReportStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 🔹 UX (manejo de rangos seguro)
        public int FromItem => TotalItems == 0 
            ? 0 
            : (CurrentPage - 1) * PageSize + 1;

        public int ToItem => TotalItems == 0 
            ? 0 
            : Math.Min(CurrentPage * PageSize, TotalItems);
    }
}