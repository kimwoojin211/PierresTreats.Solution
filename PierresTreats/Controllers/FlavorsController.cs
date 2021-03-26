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
  public class FlavorsController : Controller
  {
    private readonly PierresTreatsContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FlavorsController(UserManager<ApplicationUser> userManager, PierresTreatsContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    [AllowAnonymous]
    public ActionResult Index()
    {
      return View(_db.Flavors.ToList());
    }

    public ActionResult Create()
    {
      ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Flavor flavor, int TreatId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      flavor.User = currentUser;
      _db.Flavors.Add(flavor);
      _db.SaveChanges();

      if (TreatId != 0)
      {
        _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = flavor.FlavorId, TreatId = TreatId });
      }

      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Details(int flavorId)
    {
      Flavor thisFlavor = _db.Flavors.Include(flavor => flavor.JoinEntities).ThenInclude(join => join.Treat).FirstOrDefault(flavor => flavor.FlavorId == flavorId);
      return View(thisFlavor);
    }

  }
}