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
      if (treat.Name is null)
      {
        ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Type");
        ViewBag.ErrorMessage = "Please enter the new treat's name";
        return View(treat);
      }
      else
      {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        treat.UserCreated = currentUser.ToString();
        treat.User = currentUser;
        _db.Treats.Add(treat);
        _db.SaveChanges();

        if (FlavorId != 0)
        {
          _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
        }
      }

      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public ActionResult Details(int treatId)
    {
      Treat thisTreat = _db.Treats
        .Include(treat => treat.JoinEntities)
        .ThenInclude(join => join.Flavor)
        .FirstOrDefault(treat => treat.TreatId == treatId);
      return View(thisTreat);
    }

    public ActionResult Edit(int treatId)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == treatId);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View(thisTreat);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(Treat treat)
    {
      if (treat.Name is null)
      {
        ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
        ViewBag.ErrorMessage = "Please enter the new treat's name";
        return View(treat);
      }
      else
      {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        treat.User = currentUser;

        _db.Entry(treat).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Details", new { treatId = treat.TreatId});
      }
    }

    public ActionResult Delete(int treatId)
    {
      Treat thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == treatId);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int treatId)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == treatId);
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteFlavor(int joinId)
    { 
      var joinEntry = _db.FlavorTreat.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      int treatId = joinEntry.Treat.TreatId;
      _db.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { treatId = treatId });
    }

    public ActionResult AddFlavor(int treatId)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == treatId);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Type");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult AddFlavor(Treat treat, int FlavorId)
    {
      bool match = _db.FlavorTreat.Any(JoinEntity => JoinEntity.FlavorId == FlavorId && JoinEntity.TreatId == treat.TreatId);
      if (!match && FlavorId != 0)
      {
        _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = treat.TreatId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { treatId = treat.TreatId });
    }
  }
}