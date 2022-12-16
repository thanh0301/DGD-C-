using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGD.Models
{
    public class Giohang
    {

        dbQLDGDDataContext data=new dbQLDGDDataContext();

        public int iSanP { set; get; }
        public string sTenSanPham { set; get; }
        public string sAnh { set; get; }

        public Double dDongia { set; get; }

        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public Giohang(int MaSP)
        {
            iSanP = MaSP;
            SanPham sp = data.SanPhams.Single(n => n.MaSP == iSanP);
            sTenSanPham = sp.TenSp;
            sAnh = sp.anh;
            dDongia = double.Parse(sp.GiaBan.ToString());
            iSoluong = 1;
        }

    }
}