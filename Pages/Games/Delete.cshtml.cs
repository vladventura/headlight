using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Pages.Games
{
    public class DeleteModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int GameId { get; set; }
        public Game? SelectedGame {  get; set; }
        public void OnGet()
        {
            SelectedGame = context.Games.Where(g => g.Id == GameId).FirstOrDefault();
        }

        public IActionResult OnPostConfirmDelete()
        {
            SelectedGame = context.Games.Where(g => g.Id == GameId).FirstOrDefault();
            if (SelectedGame  != null)
            {
                context.Games.Remove(SelectedGame);
                context.SaveChanges();
            }
            return RedirectToPage("/Games/Index");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Games/Index");
        }
    }
}
