using lab2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace lab2.Controllers
{
    public class HomeController : Controller
    {
        static NoteContext db = new NoteContext();
        List<Note> allNotes = db.Notes.OrderBy(a => a.Time).ToList<Note>();

        [Authorize(Roles = "user")]
        public ActionResult Search()
        {          
            return View(allNotes);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public ActionResult Search(string key)
        {
            ViewBag.Key = key;
            Regex regex = new Regex($@"(\.*){key}(\.*)");
            List<Note> keyNotes = new List<Note>();
            foreach (Note note in allNotes)
            {
                if (regex.IsMatch(note.Message))
                {
                    keyNotes.Add(note);
                }                
            }
            return View(keyNotes);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Personal()
        {
            return View(allNotes);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(Note note)
        {
            note.Time = DateTime.Now;
            db.Notes.Add(note);
            db.SaveChanges();

            return RedirectToAction("Personal");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            Note note = db.Notes.Find(id);
            return View(note);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Note note)
        {
            Note oldNote = db.Notes.Find(note.Id);
            oldNote.Title = note.Title;
            oldNote.Message = note.Message;
            oldNote.Time = DateTime.Now;
            db.Entry(oldNote).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Personal");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            Note note = db.Notes.Find(id);
            if (note != null)
            {
                db.Notes.Remove(note);
                db.SaveChanges();
            }
            return RedirectToAction("Personal");
        }
    }
}