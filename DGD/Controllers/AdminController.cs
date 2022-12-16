using DGD.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DGD.Controllers
{
    public class AdminController : Controller
    {
        dbQLDGDDataContext data = new dbQLDGDDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Chưa điền tên người dùng!!!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu!!!";
            }
            else
            {
                Admin ad = data.Admins.SingleOrDefault(n => n.taikhoan == tendn && n.matkhau == matkhau);
                if (ad != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Sai tên đăng nhập hoặc mật khẩu";
            }
            return View();
        }

        public ActionResult SanPham(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.SanPhams.ToList().OrderBy(n => n.Ngay).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DanhMuc(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.DanhMucs.ToList().OrderBy(n => n.MaDM).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult LoaiSanPham(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.LoaiSPs.ToList().OrderBy(n => n.MaLsp).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemDM()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemDM(DanhMuc dm)
        {
            data.DanhMucs.InsertOnSubmit(dm);
            data.SubmitChanges();
            return RedirectToAction("DanhMuc");
        }

        [HttpGet]
        public ActionResult ThemSP()
        {
            ViewBag.MaLsp = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.LoaiSP1), "MaLsp", "LoaiSP1");
            ViewBag.MaNH = new SelectList(data.NhanHieus.ToList().OrderBy(n => n.TenNH), "MaNH", "TenNH");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSP(SanPham sp, HttpPostedFileBase fileupload)
        {
            ViewBag.MaLsp = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.LoaiSP1), "MaLsp", "LoaiSP1");
            ViewBag.MaNH = new SelectList(data.NhanHieus.ToList().OrderBy(n => n.TenNH), "MaNH", "TenNH");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng thêm ảnh sản phẩm";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/image"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    sp.anh = fileName;
                    data.SanPhams.InsertOnSubmit(sp);
                    data.SubmitChanges();
                }
            }
            return RedirectToAction("SanPham");
        }
        [HttpGet]
        public ActionResult ThemLSP()
        {
            ViewBag.MaDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDM), "MaDM", "TenDM");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemLSP(LoaiSP lsp)
        {
            ViewBag.MaDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDM), "MaDM", "TenDM");
            data.LoaiSPs.InsertOnSubmit(lsp);
            data.SubmitChanges();
            return RedirectToAction("LoaiSanPham");
        }
        public ActionResult xoadh(int id)
        {
            SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }
        [HttpPost, ActionName("xoadh")]
        public ActionResult Xacnhanxoa(int id)
        {
            SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SanPhams.DeleteOnSubmit(sanpham);
            data.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        public ActionResult xoadm(int id)
        {
            DanhMuc danhmuc = data.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            ViewBag.MaDM = danhmuc.MaDM;
            if(danhmuc == null)
            {
                Response.StatusCode = 404;
                return null ;
            }
            return View(danhmuc);
        }
        [HttpPost, ActionName("xoadm")]
        public ActionResult Xacnhanxoadm(int id)
        {
            DanhMuc danhmuc = data.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            if (danhmuc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.DanhMucs.DeleteOnSubmit(danhmuc);
            data.SubmitChanges();
            return RedirectToAction("DanhMuc");
        }
        public ActionResult xoalsp(int id)
        {
            LoaiSP lsp = data.LoaiSPs.SingleOrDefault(n => n.MaLsp == id);
            ViewBag.MaLsp = lsp.MaLsp;
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lsp);
        }
        [HttpPost, ActionName("xoalsp")]
        public ActionResult Xacnhanxoalsp(int id)
        {
            LoaiSP lsp = data.LoaiSPs.SingleOrDefault(n => n.MaLsp == id);
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.LoaiSPs.DeleteOnSubmit(lsp);
            data.SubmitChanges();
            return RedirectToAction("LoaiSanPham");
        }

        public List<DatHang> LayDonHang()
        {
            return data.DatHangs.ToList();
        }

        public List<CHITIETDONHANG> LayCTDonHang(int madh)
        {
            return data.CHITIETDONHANGs.Where(n => n.MaDH == madh).ToList();
        }
        public ActionResult QLDonHang()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            List<DatHang> lstDonHang = LayDonHang();
            return View(lstDonHang);
        }

        public ActionResult QLCTDH(int madh)
        {
            List<CHITIETDONHANG> ctdh = LayCTDonHang(madh);
            ViewBag.TongTien = TongTien(madh);
            return View(ctdh);
        }

        public double TongTien(int madh)
        {
            double tt = 0;
            List<CHITIETDONHANG> ctdh = LayCTDonHang(madh);
            if (ctdh != null)
            {
                tt = ctdh.Sum(n => n.Soluong * n.SanPham.GiaBan).Value;
            }
            return tt;
        }
        public ActionResult CapNhatThanhToan(int madh)
        {
            List<DatHang> lstDonHang = LayDonHang();

            DatHang dh = lstDonHang.SingleOrDefault(n => n.MaDH == madh);
            if (dh != null)
            {
                dh.Dathanhtoan = true;
            }
            UpdateModel(dh);
            data.SubmitChanges();
            return RedirectToAction("QLDonHang");
        }
        public ActionResult CapNhatGiaoHang(int madh)
        {
            List<DatHang> lstDonHang = LayDonHang();

            DatHang dh = lstDonHang.SingleOrDefault(n => n.MaDH == madh);
            if (dh != null)
            {
                dh.Tinhtranggiaohang = true;
            }
            UpdateModel(dh);
            data.SubmitChanges();
            return RedirectToAction("QLDonHang");
        }
    }
}