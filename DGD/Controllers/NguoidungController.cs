using DGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace DGD.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        private static string urlAfterLogin; // lưu lại link đang ở trước khi nhấn đăng nhập
        private static readonly int CHECK_USERNAME = 1;
        private static readonly int CHECK_EMAIL = 2;
        dbQLDGDDataContext data = new dbQLDGDDataContext();

        /*gửi email*/
        public static void SendEmail(string address, string subject, string message)
        {
            if (new EmailAddressAttribute().IsValid(address)) // check có đúng mail khách hàng
            {
                string email = "inazumaqiqi@gmail.com";
                var senderEmail = new MailAddress(email, "Teacup Shoes Store (tin nhắn tự động)");
                var receiverEmail = new MailAddress(address, "Receiver");
                var password = "fxjfpulvdnahfmpj";
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
            }
        }
        /* má hóa MD5*/
        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string strRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(strRegex);

            return regex.IsMatch(email);
        }
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }

        private Boolean checkUser(string str, int value)
        {

            if (value == 1)
            {
                var a = data.KhachHangs.FirstOrDefault(p => p.TaiKhoan == str);
                if (a != null) return true;
            }
            else if (value == 2)
            {
                var a = data.KhachHangs.FirstOrDefault(p => p.Email == str);
                if (a != null) return true;
            }
            return false;
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhau1 = collection["MatKhau"];
            var matkhaunhaplai = collection["MatKhaunhaplai"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];

            if (String.IsNullOrEmpty(hoten) || String.IsNullOrEmpty(matkhau) || String.IsNullOrEmpty(matkhau1) ||
                 String.IsNullOrEmpty(matkhaunhaplai) || String.IsNullOrEmpty(email) ||
                 String.IsNullOrEmpty(diachi) || String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ nội dung";
                return this.Dangky();
            }
            else if (checkUser(tendn, CHECK_USERNAME))  // kiểm tra tài khoản đã tồn tại hay chưa
            {
                ViewData["Error"] = "Tài khoản đã tồn tại";
                return this.Dangky();
            }
            else if (checkUser(email, CHECK_EMAIL))  // kiểm tra tài khoản đã tồn tại hay chưa
            {
                ViewData["Error"] = "Email đã tồn tại";
                return this.Dangky();
            }
        
            else if (!String.Equals(matkhau.ToString(), matkhaunhaplai.ToString()))
            {
                ViewData["Error"] = "Mật khẩu không khớp";
                return this.Dangky();
            }
            else
            {
                kh.TenKH = hoten;
                kh.TaiKhoan = tendn;
                kh.MatKhau = EncodePassword(matkhau);
                kh.MatKhau1 = matkhau;
                kh.Email = email;
                kh.DiaChi = diachi;
                kh.SDT = dienthoai;
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();
                string subject = "Chào Mừng Đến Với web đồ gia dụng ";
                string message = "Chào mừng " + kh.TenKH + " đến với đồ gia dụng, Chúc bạn có một ngày vui vẻ. Bé tâm đáng yêu cute hột me";
                SendEmail(email, subject, message); // gửi mail chào mừng
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        } 

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]  
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["loi1"] = "Phải nhâp tên đắng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["loi2"] = "Phải nhâp mat khau";
            }
            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == EncodePassword(matkhau));
                if (kh != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Trangchu", "Home");
                }
                else

                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        [HttpGet]
        public ActionResult quenMK()
        {
            return View();
        }
        [HttpPost]
        public ActionResult quenMK(FormCollection collection)
        {
            var email = collection["TenDN1"];
            if (String.IsNullOrEmpty(email))
            {
                ViewData["loi1"] = "Phải nhâp email";
            }

            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.Email == email);
                if (kh != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    string subject = "Xác nhận thành công !";
                    string message = "Mật khẩu cũ " + kh.MatKhau1 + " đã trở lại ! ";
                    SendEmail(email, subject, message);
                    return RedirectToAction("Dangnhap", "Nguoidung");
                }

            }
            return View();
        }
        
        public ActionResult LogOff()
        {
            Session["TaiKhoan"] = null;

            return RedirectToAction("Trangchu", "Home");
        }
        [HttpGet]
        public ActionResult MyProfile() // chuyển đến trang hồ sơ
        {
            if (Session["TaiKhoan"] == null) RedirectToAction("Login");
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            return View(kh);
        }
        [HttpPost]
        public ActionResult MyProfile(string strURL, FormCollection collection)
        {

            if (Session["TaiKhoan"] == null) RedirectToAction("Dangnhap");
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
           
            var user = data.KhachHangs.SingleOrDefault(p => p.MaKH == kh.MaKH);

            var name = collection["name"];
            var email = collection["email"];
            var address = collection["address"];
            var phoneNumber = collection["phoneNumber"];
          

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(email) || // kiểm tra null
                String.IsNullOrEmpty(address) || String.IsNullOrEmpty(phoneNumber))
            {
                ViewData["Error"] = "Vui lòng điền đủ thông tin";
                return this.MyProfile();
            }
            user.TenKH = name;
            user.Email = email;
            user.DiaChi = address;
            user.SDT = phoneNumber;
            data.SubmitChanges();
            Session["TaiKhoan"] = user;
            ViewData["Success"] = "Cập nhật thành công";
            return this.MyProfile();
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection collection)
        {
            if (Session["TaiKhoan"] == null) RedirectToAction("Login");
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
           
            var user = data.KhachHangs.SingleOrDefault(p => p.MaKH == kh.MaKH);

            var oldPassword = collection["oldPassword"];
            var newPassword = collection["newPassword"];
            var confirmNewPassword = collection["confirmNewPassword"];

            if (String.IsNullOrEmpty(oldPassword) || String.IsNullOrEmpty(newPassword) || // trống textbox
                String.IsNullOrEmpty(confirmNewPassword))
            {
                ViewData["Error"] = "Vui lòng điền đủ thông tin";
                return this.ChangePassword();
            }
            else if (!String.Equals(newPassword, confirmNewPassword)) // 2 ô mật khẩu mới không khớp
            {
                ViewData["Error"] = "Mật khẩu mới không khớp";
                return this.ChangePassword();
            }
            else if (!String.Equals(EncodePassword(oldPassword), user.MatKhau)) // kiểm tra mật khẩu cũ
            {
                ViewData["Error"] = "Sai mật khẩu cũ";
                return this.ChangePassword();
            }
            else // ==============thay đổi mật khẩu===================
            {
                newPassword = EncodePassword(newPassword);
                user.MatKhau = newPassword;
                data.SubmitChanges();
                Session["TaiKhoan"] = user;
                ViewData["Success"] = "Đổi mật khẩu thành công";
                return this.ChangePassword();
            }
        }
        public List<DatHang> LayDonHang()
        {
            var taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return null;
            }
            return data.DatHangs.Where(n => n.MaKH == taikhoan.MaKH).ToList();
        }

        public List<CHITIETDONHANG> LayCTDonHang(int madh)
        {
            var taikhoan = (KhachHang)Session["TaiKhoan"];
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return null;
            }
            return data.CHITIETDONHANGs.Where(n => n.MaDH == madh).ToList();
        }
        public ActionResult QLDonHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            List<DatHang> lstDonHang = LayDonHang();
            return View(lstDonHang);
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
        public ActionResult QLCTDH(int madh)
        {
            List<CHITIETDONHANG> ctdh = LayCTDonHang(madh);
            ViewBag.TongTien = TongTien(madh);
            return View(ctdh);
        }

    }
}