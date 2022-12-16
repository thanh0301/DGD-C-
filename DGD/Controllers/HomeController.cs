using DGD.Models;
using Microsoft.Ajax.Utilities;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using DGD.MultipleModelInOneView;

namespace DGD.Controllers
{
    public class HomeController : Controller
    {

        dbQLDGDDataContext data = new dbQLDGDDataContext();

        public ActionResult Trangchu()
        {
            return View();
        }
        //Lấy sản phẩm

        private List<SanPham> LaySP(int count, int lsp, int nh)
        {
            if (nh == 0)
                return data.SanPhams.Where(a => a.MaLsp == lsp).Take(count).ToList();
            return data.SanPhams.Where(a => a.MaLsp == lsp && a.MaNH == nh).Take(count).ToList();
        }

        public ActionResult SPtheoLoai(int ? page, int lsp, int nh)
        {
            int pageSize = 16;
            int pageNum = (page ?? 1);
            var sp = LaySP(70, lsp, nh);
            return View (sp.ToPagedList(pageNum,pageSize));
        }

        public ActionResult Detail(int id)
        {
            var sp = from s in data.SanPhams
                     where s.MaSP == id
                     select s;
            return View(sp.Single());
        }

        public ActionResult Search(string searching)
        {
            //Lấy ra danh sách sản phẩm từ chuỗi tìm kiếm truyền vào
            var sp = from s in data.SanPhams select s;
            ViewBag.TuKhoa = searching;

            return View(data.SanPhams.Where(x => x.TenSp.Contains(searching) || searching == null).ToList());
        }

        private List<SanPham> LaySanPhamTheoLoai(int id, int count)
        {
            //Sắp xếp giảm dần theo ngày cập nhật, lấy "count" dòng đầu
            return data.SanPhams.Where(a => a.LoaiSP.MaLsp == id).Take(count).ToList();
        }
        public ActionResult subProduct(int id, int count)
        {
            var sanphams = LaySanPhamTheoLoai(id, count);

            var rnd = new Random();
            var spRandom = sanphams.OrderBy(item => rnd.Next());
            return PartialView(spRandom);
        }
        public ActionResult ListSPDanhMuc(int id, int? page)
        {
            //Page List
            int pageSize = 20;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            var sanPham = from loaiSp in data.LoaiSPs
                          from sp in data.SanPhams
                          where loaiSp.MaDM == id && sp.MaLsp == loaiSp.MaLsp
                          select sp;

            //Lấy Ảnh (Đã Optimize)


            //Lấy Title
            DanhMuc tempDM = data.DanhMucs.SingleOrDefault(n => n.MaDM == id);
            ViewBag.Name = tempDM.TenDM;

            return View(sanPham.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Chude()

        {
            ModelDanhMucs modelDanhMucs = new ModelDanhMucs();

            modelDanhMucs.DANHMUCs = from cd in data.DanhMucs select cd;
            modelDanhMucs.LOAISANPHAMs = from lsp in data.LoaiSPs select lsp;



            return PartialView(modelDanhMucs);

        }

        public ActionResult ChudeonMB()
        {

            ModelDanhMucs modelDanhMucsMb = new ModelDanhMucs();
            modelDanhMucsMb.DANHMUCs = from cd in data.DanhMucs select cd;
            modelDanhMucsMb.LOAISANPHAMs = from lsp in data.LoaiSPs select lsp;

            return PartialView(modelDanhMucsMb);

        }
        public ActionResult ListSPLoai(int id, int? page)
        {
            //Page List
            int pageSize = 20;
            //Tạo biến số trang
            int pageNum = (page ?? 1);

            var sanPham = from loaiSp in data.LoaiSPs
                          from sp in data.SanPhams
                          where loaiSp.MaLsp == id && sp.MaLsp == loaiSp.MaLsp
                          select sp;


            //Lấy Title
            LoaiSP tempLoai = data.LoaiSPs.SingleOrDefault(n => n.MaLsp == id);
            ViewBag.Name = tempLoai.DanhMuc.TenDM;
            ViewBag.NameLoai = tempLoai.LoaiSP1;

            return View(sanPham.ToPagedList(pageNum, pageSize));
        }
        public ActionResult DanhMuc(int? page)
        {
            //if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            //{
            //    return RedirectToAction("Login", "Admin");
            //}
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.DanhMucs.ToList().OrderBy(n => n.MaDM).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult ThemDM()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemDM(DanhMuc dm, HttpPostedFileBase fileUpload)
        {
            data.DanhMucs.InsertOnSubmit(dm);
            data.SubmitChanges();
            return RedirectToAction("DanhMuc");
        }


        private List<SanPham> laytivisamsung(int count)
        {

            return data.SanPhams.Where(a => a.MaNH == (1) && a.MaLsp == (1)).Take(count).ToList();
        }
        private List<SanPham> laytiviSony(int count)
        {

            return data.SanPhams.Where(a => a.MaNH == (2) && a.MaLsp == (2)).Take(count).ToList();
        }
        private List<SanPham> laytiviLG(int count)
        {

            return data.SanPhams.Where(a => a.MaNH == (3) && a.MaLsp == (3)).Take(count).ToList();
        }
        public ActionResult SPTiviSony(int? page)
        {
            int pageSize = 16;
            int pageNum = (page ?? 1);
            var sp = laytiviSony(70);
            return View(sp.ToPagedList(pageNum, pageSize));
        }
        public ActionResult SPTiviLG(int? page)
        {
            int pageSize = 16;
            int pageNum = (page ?? 1);
            var sp = laytiviLG(70);
            return View(sp.ToPagedList(pageNum, pageSize));
        }
        public ActionResult SPTivisamsung(int? page)
        {
            int pageSize = 16;
            int pageNum = (page ?? 1);
            var sp = laytivisamsung(70);
            return View(sp.ToPagedList(pageNum, pageSize));
        }


    }
}