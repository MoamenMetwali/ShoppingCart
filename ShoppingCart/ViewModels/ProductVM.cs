﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.Models;

namespace Store.ViewModels
{
    public class ProductVM
    { 
        public Product Product { get; set; }
        public SelectList CatList { get; set; }
    }
}
