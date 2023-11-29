using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PermissionsController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("PermissionEdit")]
        public IHttpActionResult PermissionEdit(int id)
        {
            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Permissions permission = db.BB_Permissions.Where(x => x.ID == id).FirstOrDefault();

                    return Ok(permission);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("SavePermissions")]
        public IHttpActionResult SavePermissions(BB_Permissions permissions)
        {
            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    permissions.CreatedTime = DateTime.Now;
                    permissions.ModifiedTime = DateTime.Now;
                    db.BB_Permissions.Add(permissions);

                    db.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("SavePermission_Edit")]
        public IHttpActionResult SavePermission_Edit(BB_Permissions permissions)
        {
            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Permissions p = db.BB_Permissions.Where(x => x.ID == permissions.ID).FirstOrDefault();

                    if (p != null)
                    {
                        p.EndDate = permissions.EndDate;
                        p.InitialDate = permissions.InitialDate;
                        p.IsPermanent = permissions.IsPermanent;
                        p.UserEmaill = permissions.UserEmaill;
                        p.ModifiedTime = DateTime.Now;
                        db.Entry(p).State = EntityState.Modified;
                        db.SaveChanges();

                    }


                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DeletePermission")]
        public IHttpActionResult DeletePermission(int id)
        {
            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Permissions permission = db.BB_Permissions.Where(x => x.ID == id).FirstOrDefault();

                    db.BB_Permissions.Remove(permission);
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetListPermissions")]
        public IHttpActionResult GetListPermissions(ProfileUser userName)
        {
            List<BB_Permissions> listModel = new List<BB_Permissions>();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    listModel = db.BB_Permissions.Where(x => x.CreatedUser == userName.Username).ToList();

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetListPermissionsHistory")]
        public IHttpActionResult GetListPermissionsHistory(ProfileUser userName)
        {
            List<BB_Permissions_History> listModel = new List<BB_Permissions_History>();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    listModel = db.BB_Permissions_History.Where(x => x.CreatedUser == userName.Username).ToList();

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }


    }
}
