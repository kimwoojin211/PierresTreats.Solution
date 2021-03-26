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
      if (flavor.Type is null)
      {
        ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Name");
        ViewBag.ErrorMessage = "Please enter a flavor.";
        return View();
      }
      else
      {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        flavor.UserCreated = currentUser.ToString();
        flavor.User = currentUser;
        _db.Flavors.Add(flavor);
        _db.SaveChanges();
      }

      if (TreatId != 0)
      {
        _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = flavor.FlavorId, TreatId = TreatId });
      }

      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public ActionResult Details(int flavorId)
    {
      Flavor thisFlavor = _db.Flavors
        .Include(flavor => flavor.JoinEntities)
        .ThenInclude(join => join.Treat)
        .FirstOrDefault(flavor => flavor.FlavorId == flavorId);
      return View(thisFlavor);
    }

    public ActionResult Edit(int flavorId)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == flavorId);

      ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Name");
      return View(thisFlavor);
    }
    
    [HttpPost]
    public async Task<ActionResult> Edit(Flavor flavor)
    {
      if (flavor.Type is null)
      {
        ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Name");
        ViewBag.ErrorMessage = "Please enter a name.";
        return View(flavor);
      }
      else
      {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        flavor.User = currentUser;
        _db.Entry(flavor).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Details", new { flavorId = flavor.FlavorId });
      }
    }

    public ActionResult Delete(int flavorId)
    {
      Flavor thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == flavorId);
      return View(thisFlavor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int flavorId)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == flavorId);
      _db.Flavors.Remove(thisFlavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteTreat(int joinId)
    {
      var joinEntry = _db.FlavorTreat.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      int flavorId = joinEntry.Flavor.FlavorId;
      _db.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { flavorId = flavorId });
    }

    public ActionResult AddTreat(int flavorId)
    {
      var thisFlavor = _db.Flavors.FirstOrDefault(flavor => flavor.FlavorId == flavorId);
      ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Name");
      return View(thisFlavor);
    }

    [HttpPost]
    public ActionResult AddTreat(Flavor flavor, int TreatId)
    {
      bool match = _db.FlavorTreat.Any(JoinEntity => JoinEntity.FlavorId == flavor.FlavorId && JoinEntity.TreatId == TreatId);
      {
        if (!match && TreatId != 0)
        {
          _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = flavor.FlavorId, TreatId = TreatId });
          _db.SaveChanges();
        }
      }
      return RedirectToAction("Details", new { flavorId = flavor.FlavorId });
    }
  }
}