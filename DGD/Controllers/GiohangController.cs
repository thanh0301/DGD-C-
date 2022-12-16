using DGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DGD.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        dbQLDGDDataContext data = new dbQLDGDDataContext();
        public List<GioHang> Laygiohang()
        {
            var taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return null;
            }
            return data.GioHangs.Where(n => n.MaKH == taikhoan.MaKH).ToList();
        }

        public ActionResult ThemGiohang(int iSanP, string strURL)
        {
            KhachHang taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == iSanP && n.MaKH == taikhoan.MaKH);
            if (sanpham == null)
            {
                sanpham = new GioHang();
                sanpham.MaSP = iSanP;
                sanpham.MaKH = taikhoan.MaKH;
                sanpham.SL = 1;
                data.GioHangs.InsertOnSubmit(sanpham);
            }
            else
            {
                sanpham.SL++;
                UpdateModel(sanpham);
            }
            data.SubmitChanges();
            return Redirect(strURL);
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Laygiohang();
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.SL).Value;
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Laygiohang();
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.SL*n.SanPham.GiaBan).Value;
            }
            return iTongTien;
        }
        public ActionResult Giohang()
        {
            var taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int iMaSp)
        {
            List<GioHang> lstGiohang = Laygiohang();

            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == iMaSp);

            if (sanpham != null)
            {
                data.GioHangs.DeleteOnSubmit(sanpham);
                data.SubmitChanges();
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Trangchu", "Home");
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult CapnhatGioHang(int iMaSp, FormCollection f)
        {
            List<GioHang> lstGiohang = Laygiohang();

            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == iMaSp);

            if (sanpham != null)
            {
                sanpham.SL = int.Parse(f["txtSoluong"].ToString());
            }
            UpdateModel(sanpham);
            data.SubmitChanges();
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            data.GioHangs.DeleteAllOnSubmit(lstGiohang);
            data.SubmitChanges();
            return RedirectToAction("Trangchu", "Home");
        }

        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DatHang dh = new DatHang();
            KhachHang kh = (KhachHang)Session["Taikhoan"];
            List<GioHang> gh = Laygiohang();
            dh.MaKH = kh.MaKH;
            dh.NgayDat = DateTime.Today;
            dh.Ngaygiao = DateTime.Today.AddDays(7);
            dh.Tinhtranggiaohang = false;
            dh.Dathanhtoan = false;
            data.DatHangs.InsertOnSubmit(dh);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                CHITIETDONHANG ctdh = new CHITIETDONHANG();
                ctdh.MaDH = dh.MaDH;
                ctdh.MaSP = item.MaSP;
                ctdh.Soluong = item.SL;
                data.CHITIETDONHANGs.InsertOnSubmit(ctdh);
                data.GioHangs.DeleteOnSubmit(item);
            }
            data.SubmitChanges();
            Session["giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");

        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}