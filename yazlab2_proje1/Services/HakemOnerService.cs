using System.Linq;
using makalesistemi.Models;

namespace makalesistemi.Services
{

    public class HakemOnerService
    {
        private readonly Context _context;
        private readonly MakaleAnalizService _makaleAnalizService;

        public HakemOnerService(Context context, MakaleAnalizService makaleAnalizService)
        {
            _context = context;
            _makaleAnalizService = makaleAnalizService;
        }

       /*ublic Hakem? OnerHakem(string makaleIcerik)
        {
            string konu = _makaleAnalizService.BelirleMakaleKonusu(makaleIcerik);

             return _context.Hakemler.FirstOrDefault(h => h.UzmanlikAlani == konu);
            
        }*/
    }

}
