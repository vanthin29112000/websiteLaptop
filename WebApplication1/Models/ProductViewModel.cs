using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class ProductFilterViewModeltest
    {
        public Boolean IsUnder10MillionChecked { get; set; }

        public Boolean Is10To15MillionChecked { get; set; }
        public Boolean Is15To20MillionChecked { get; set; }

        public Boolean Is20To30MillionChecked { get; set; }

        public Boolean Is30To50MillionChecked { get; set; }
        public Boolean Is50To100MillionChecked { get; set; }



        public List<string> SelectedSizeScreen { get; set; } = new List<string>();
        public List<string> SelectedScreenTechnology { get; set; } = new List<string>();
        public List<string> SelectedResolution { get; set; } = new List<string>();
        public List<string> SelectedCPU { get; set; } = new List<string>();
        public List<string> SelectedRAM { get; set; } = new List<string>();
        public string SortColumn { get; set; }
        // Thuộc tính tìm kiếm
        public string SearchQuery { get; set; }
        public List<Product> Products { get; set; }
    }
}