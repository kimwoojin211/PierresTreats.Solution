using Microsoft.AspNetCore.Mvc;
using PierresTreats.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PierresTreats.Controllers
{
  [Authorize]
  public class TreatsController : Controller
  {
    private readonly PierresTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TreatsController(UserManager<ApplicationUser> userManager, PierresTreatsContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      return View(_db.Treats.ToList());
    }
    public ActionResult Create()
    {
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Type");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Treat treat, int FlavorId)
    {
      // gonna save this user value for displaying who created/last edited
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      treat.User = currentUser;
      _db.Treats.Add(treat);
      _db.SaveChanges();

      if (FlavorId != 0)
      {
        _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
      }

      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Details(int treatId)
    {
      Treat thisTreat = _db.Treats.Include(treat => treat.JoinEntities).ThenInclude(join => join.Flavor).FirstOrDefault(treat => treat.TreatId == treatId);
      return View(thisTreat);
    }

  }
}