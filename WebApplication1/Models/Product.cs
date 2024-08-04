﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public int Id { get; set; }
        public string Technology { get; set; }
        public string NumberOfCores { get; set; }
        public string NumberOfThreads { get; set; }
        public string CPUSpeed { get; set; }
        public string MaxSpeed { get; set; }
        public string CacheSize { get; set; }
        public string RAM { get; set; }
        public string RAMType { get; set; }
        public string RAMBusSpeed { get; set; }
        public string MaxRAMSupport { get; set; }
        public string Storage { get; set; }
        public string Screen { get; set; }
        public string Resolution { get; set; }
        public string ScreenTechnology { get; set; }
        public string GraphicsCard { get; set; }
        public string AudioTechnology { get; set; }
        public string PortsInfo { get; set; }
        public string WirelessConnectivity { get; set; }
        public string Webcam { get; set; }
        public string DimensionsAndWeight { get; set; }
        public string BatteryInfo { get; set; }
        public string KeyboardBacklight { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public Nullable<int> CurrentPrice { get; set; }
        public Nullable<int> OldPrice { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Description { get; set; }
        public string ImageLinks { get; set; }
        public string ScanFrequency { get; set; }
        public string MemoryCard { get; set; }
        public string OtherFunction { get; set; }
        public string Material { get; set; }
        public string OperatingSystem { get; set; }
        public string LaunchTime { get; set; }
    }

    public class ProductFilterViewModel
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
        public string SelectedBrand { get; set; }
        // Thuộc tính tìm kiếm
        public string SearchQuery { get; set; }
        public List<Product> Products { get; set; }
    }
}
