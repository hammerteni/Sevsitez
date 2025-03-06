using iTextSharp.text;
using site.dbContext;
using site.models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace site.controllers
{
    public class EducationOrganizationsController : ApiController
    {
        private EducationOrgAreasContext db;
        private CompetitionsContext dbRequests;

        public EducationOrganizationsController() {
            db = new EducationOrgAreasContext();
            dbRequests = new CompetitionsContext();
        }

        // GET: api/EducationOrganizations?page=0&size=10
        [HttpGet]
        [ResponseType(typeof(EducationOrganizations))]
        public async Task<IHttpActionResult> GetEducationOrganizations(string searchString, int? page, int? size)
        {
            if (HttpContext.Current.Session == null || (HttpContext.Current.Session != null && HttpContext.Current.Session["authperson"] == null))
                return StatusCode(HttpStatusCode.Unauthorized);

            var result = await db.EducationOrganization.Where(x => string.IsNullOrEmpty(searchString) || 
                            (DbFunctions.Like(x.FullName.ToLower(), "%" + searchString.ToLower() + "%")
                            || DbFunctions.Like(x.Name.ToLower(), "%" + searchString.ToLower() + "%")))
                        .ToListAsync();
            var countItems = result.Count();
            var educationOrganizations = result
                                        .OrderBy(p => p.Name) 
                                        .Skip((int)((page ?? 0) * size))
                                        .Take((int)(size ?? 0))
                                        .ToList();
            
            return Json(new { educationOrganizations, countItems });
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ModifyEducationOrganizations(string fullName, string shortName, EducationOrganizations educationOrganizations)
        {
            if (HttpContext.Current.Session == null || (HttpContext.Current.Session != null && HttpContext.Current.Session["authperson"] == null))
                return StatusCode(HttpStatusCode.Unauthorized);

            if (!ModelState.IsValid || 
                string.IsNullOrEmpty(educationOrganizations.FullName) || string.IsNullOrEmpty(educationOrganizations.Name) || 
                string.IsNullOrEmpty(educationOrganizations.District) || string.IsNullOrEmpty(educationOrganizations.Region) || 
                string.IsNullOrEmpty(educationOrganizations.Area) || string.IsNullOrEmpty(educationOrganizations.City) || 
                string.IsNullOrEmpty(educationOrganizations.TypeName) || string.IsNullOrEmpty(educationOrganizations.MRSD)
            )
            {
                return BadRequest(ModelState);
            }
            if (fullName != educationOrganizations.FullName && shortName != educationOrganizations.Name)
            {
                return BadRequest();
            }

            db.Entry(educationOrganizations).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationOrganizationsExists(fullName, shortName))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbEntityValidationException ex)
            {
                var exstr = new StringBuilder();
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        exstr.Append("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return BadRequest(exstr.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        [HttpPost]
        [ResponseType(typeof(EducationOrganizations))]
        public async Task<IHttpActionResult> AddEducationOrganizations(EducationOrganizations educationOrganizations)
        {
            if (HttpContext.Current.Session == null || (HttpContext.Current.Session != null && HttpContext.Current.Session["authperson"] == null))
                return StatusCode(HttpStatusCode.Unauthorized);

            if (!ModelState.IsValid || 
                string.IsNullOrEmpty(educationOrganizations.FullName) || string.IsNullOrEmpty(educationOrganizations.Name) || 
                string.IsNullOrEmpty(educationOrganizations.District) || string.IsNullOrEmpty(educationOrganizations.Region) || 
                string.IsNullOrEmpty(educationOrganizations.Area) || string.IsNullOrEmpty(educationOrganizations.City) || 
                string.IsNullOrEmpty(educationOrganizations.TypeName) || string.IsNullOrEmpty(educationOrganizations.MRSD)
            )
            {
                return BadRequest(ModelState);
            }

            if (await db.EducationOrganization.AnyAsync(x => x.FullName == educationOrganizations.FullName && x.Name == educationOrganizations.Name))
            {
                return BadRequest("В базе уже есть такое учреждение " + educationOrganizations.FullName);
            }    

            db.EducationOrganization.Add(educationOrganizations);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EducationOrganizationsExists(educationOrganizations.FullName, educationOrganizations.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            catch (DbEntityValidationException ex)
            {
                var exstr = new StringBuilder();
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        exstr.Append("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return BadRequest(exstr.ToString());
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = educationOrganizations.Name }, educationOrganizations);
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> DeleteEducationOrganizations(string fullName, string shortName)
        {
            EducationOrganizations educationOrganization = await db.EducationOrganization.FirstOrDefaultAsync(x => x.FullName == fullName && x.Name == shortName);
            if (educationOrganization == null)
            {
                return NotFound();
            }

            if (dbRequests.Competitions.Any(x => x.EducationalOrganization == fullName && x.EducationalOrganizationShort == shortName)) {

                var orgs = dbRequests.Competitions.Where(x => x.EducationalOrganization == fullName && x.EducationalOrganizationShort == shortName).Select(x => x._id).ToList();
                return Ok(new Response { status = ResponseStatus.Error, 
                    message = string.Format("Нельзя удалить данное учреждение, так как есть привязанные заявки: {0}", string.Join(", ", orgs)) });
            }

            db.EducationOrganization.Remove(educationOrganization);
            await db.SaveChangesAsync();

            return Ok(educationOrganization);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EducationOrganizationsExists(string fullName, string shortName)
        {
            return db.EducationOrganization.Count(e => e.FullName == fullName && e.Name == shortName) > 0;
        }
    }
}