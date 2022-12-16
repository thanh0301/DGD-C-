using DGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGD.MultipleModelInOneView
{
    public class ModelDanhMucs
    {
        public IEnumerable<DanhMuc> DANHMUCs { get; set; }
        public IEnumerable<LoaiSP> LOAISANPHAMs { get; set; }
    }
}