
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    namespace Kendo.Controllers
    {
        public class FileController : Controller
        {
            /// <summary>
            /// unused
            /// </summary>
            /// <param name="files"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult Submit(IEnumerable<HttpPostedFileBase> files)
            {
                if (files != null)
                {
                    TempData["UploadedFiles"] = GetFileInfo(files);
                }

                return RedirectToRoute("Demo", new { section = "upload", example = "result" });
            }
            //[HttpPost]
            public ActionResult Save(IEnumerable<HttpPostedFileBase> files,string uid)
            {
                var state=ModelState.IsValid;
                // The Name of the Upload component is "files"
                if (files != null && files.ToList().Count==1)
                {
                    //foreach (var file in files)
                    //{
                        var file=files.ToList()[0];
                        var name = file.FileName;
                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName =string.Format("{0}.{1}", uid,name.Substring(name.LastIndexOf(".")+1));// Path.GetFileName(file.FileName);
                        var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                         //The files are not actually saved in this demo
                         file.SaveAs(physicalPath);
                    //}
                }

                // Return an empty string to signify success
                return Content("");
            }
            public FileResult Download(string file,string name)
            {

                byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = name;
                return response;
            }
            public ActionResult Remove(string[] fileNames,string uid)
            {
                // The parameter of the Remove action must be called "fileNames"

                if (fileNames != null && fileNames.Length==1)
               {
                    //foreach (var fullName in fileNames) zee.jpg
                    //
                        var fileName = string.Format("{0}.{1}",
                            uid,
                            fileNames[0].Substring(fileNames[0].LastIndexOf(".")+1)
                            );// Path.GetFileName(fullName);
                        var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                        // TODO: Verify user permissions

                        if (System.IO.File.Exists(physicalPath))
                        {
                            // The files are not actually removed in this demo
                             System.IO.File.Delete(physicalPath);
                        }
                    //}
               }

                // Return an empty string to signify success
                return Content("");
            }

            private IEnumerable<string> GetFileInfo(IEnumerable<HttpPostedFileBase> files)
            {
                return
                    from a in files
                    where a != null
                    select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
            }
        }
    }
