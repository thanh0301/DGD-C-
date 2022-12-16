using DGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGD.MultipleModelInOneView
{
    public class ModelSP
    {
        public IEnumerable<SanPham> SANPHAMs { get; set; }
        public IEnumerable<AnhPhu> ANHPHUs { get; set; }
    }
}