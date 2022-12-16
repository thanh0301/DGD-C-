using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DGD.Models;

namespace DGD.Controllers
{
    public class YeuThichController : Controller
    {
        dbQLDGDDataContext data = new dbQLDGDDataContext();
        // GET: YeuThich
        public List<YeuThich> LayYeuThich()
        {
            var taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return null;
            }
            return data.YeuThiches.Where(n => n.MaKH == taikhoan.MaKH).ToList();
        }

        public ActionResult ThemYeuThich(int masp,string strURL)
        {
            KhachHang taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            List<YeuThich> lstYeuThich = LayYeuThich();
            YeuThich sanpham = lstYeuThich.SingleOrDefault(n => n.MaSP == masp && n.MaKH == taikhoan.MaKH);
            if (sanpham == null)
            {
                sanpham = new YeuThich();
                sanpham.MaSP = masp;
                sanpham.MaKH = taikhoan.MaKH;
                data.YeuThiches.InsertOnSubmit(sanpham);
            }
            else
            {
                data.YeuThiches.DeleteOnSubmit(sanpham);
            }
            data.SubmitChanges();
            return Redirect(strURL);
        }
        public ActionResult YeuThich()
        {
            var taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            List<YeuThich> lstYeuThich = LayYeuThich();
            return View(lstYeuThich);
        }
        private int TongSL()
        {
            int sl = 0;
            List<YeuThich> lstYeuThich = LayYeuThich();
            if(lstYeuThich!= null)
            {
                sl = lstYeuThich.Count();
            }
            return sl;
        }
        public ActionResult YeuThichPartial()
        {
            ViewBag.TongSL = TongSL();
            return PartialView();
        }
        public ActionResult XoaYeuThich(int masp, int makh)
        {
            List<YeuThich> lstYeuThich = LayYeuThich();
            YeuThich sanpham = lstYeuThich.SingleOrDefault(n => n.MaSP == masp);
            if(sanpham!=null)
            {
                data.YeuThiches.DeleteOnSubmit(sanpham);
                data.SubmitChanges();
            }
            if(lstYeuThich.Count == 0)
            {
                return RedirectToAction("Trangchu", "Home");
            }
            return RedirectToAction("YeuThich");
        }
    }
}