using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ContainerProd.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContainerProd.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class DefaultController : Controller
    {
        private ApplicationDbContext db;

        public DefaultController(ApplicationDbContext option)
        {
            db = option;
        }
        public IActionResult Index()
        {
            return Json("Index runs");
        }


        [HttpGet]
        public ActionResult myCourses(long studentNumber, string session)
        {
            if (IsAuthenticated(studentNumber, session))
            {
                List<Course> lstCourses = new List<Course>();
                List<Register> regs = new List<Register>();
                regs = db.Registers.Where(e => e.stu_id == studentNumber && e.status == 1).ToList();
                if (regs.Count > 0)
                {
                    foreach (var reg in regs)
                    {                        
                        if(!db.Registers.Where(e=>e.stu_id==reg.stu_id && e.course_id==reg.course_id  && e.ID>reg.ID && e.status==0).Any())
                        lstCourses.Add(db.Courses.Where(e => e.ID == reg.course_id).FirstOrDefault());
                    }
                        
                }
                    
                return Json(lstCourses);
            }
            return Json("Not Authenticate");
        }


        [HttpGet]
        public ActionResult GetAllCourses(long studentNumber, string session)
        {
            string _result = string.Empty;
            if (IsAuthenticated(studentNumber, session))
            {
                List<Course> Courses = new List<Course>();
                if (db.Courses != null)
                {
                    Courses = db.Courses.ToList();
                }
                return Json(Courses);
            }
            return Json("Not Authenticate");
        }


        [HttpGet]
        public ActionResult GetCourse(long studentNumber, string session, int course_id)
        {
            string resultstatus = "Unsuccess";

            if (IsAuthenticated(studentNumber, session))
            {
                int capa = 0;
                Course entity = db.Courses.Where(e => e.ID == course_id).FirstOrDefault();
                var selected = db.Registers.Where(e => e.stu_id == studentNumber && e.course_id == course_id && e.status == 1).LastOrDefault();
                var removed = db.Registers.Where(e => e.stu_id == studentNumber && e.course_id == course_id && e.status == 0).LastOrDefault();
                if(selected!=null &&( removed==null || removed.ID<selected.ID))
                {
                    resultstatus = "Duplicate";
                    return Json(resultstatus);
                }
                if (entity != null)
                {
                    capa = entity.Capacity;
                }
                if (capa > 0)
                {
                    Register co = new Register { stu_id = studentNumber, course_id = course_id, date = DateTime.Now.ToString(), status = 1 };
                    db.Registers.Add(co);
                    try
                    {
                        db.SaveChanges();
                        entity.Capacity--;
                        db.SaveChanges();
                        resultstatus = "Success";
                    }
                    catch (Exception)
                    {
                        resultstatus = "Unsuccess";
                    }
                }
                else resultstatus = "Capacity is Zero";
                return Json(resultstatus);
            }
            else return Json("Not Authenticate");

        }

        [HttpGet]
        public ActionResult RemoveCourse(long studentNumber, string session, int course_id)
        {
            string resultstatus = "Unsuccess";
            if (IsAuthenticated(studentNumber, session))
            {
                Course entity = db.Courses.Where(e => e.ID == course_id).FirstOrDefault();
                if (entity != null)
                {
                    Register co = new Register { stu_id = studentNumber, course_id = course_id, date = DateTime.Now.ToString(), status = 0 };
                    db.Registers.Add(co);
                    try
                    {
                        db.SaveChanges();
                        entity.Capacity++;
                        db.SaveChanges();
                        resultstatus = "Success";
                    }
                    catch (Exception)
                    {
                        resultstatus = "Unsuccess";
                    }
                    return Json(resultstatus);
                }
                else return Json("Not found");
            }
            return Json("Not Authenticate");
        }

        public bool IsAuthenticated(long studentNumber, string session)
        {
            bool result;
            string urlreq = "http://auth_service:8080/authenticate?studentNumber=" + studentNumber + "&session=" + session + "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlreq);
                request.Method = "GET";
                request.KeepAlive = true;
                request.ContentType = "appication/json";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string myResponse = string.Empty;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    myResponse = sr.ReadToEnd();
                }
                Auth_Response auth_ = null;
                if (myResponse.Length > 0)
                    auth_ = myResponse.FromJson<Auth_Response>();
                if (auth_ != null && auth_.status == "200")
                    result = true;
                else result = false;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
