using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPFinalStrasbourg.Repositories;
using TPFinalStrasbourg.Services;
using TPFinalStrasbourg.Models;
using TPFinalStrasbourg.DTOs;

namespace TPFinalStrasbourg.Controllers
{
    [Route("api/v1/ads")]
    public class AdController : Controller
    {
        private JWTService _jwtService;
        private AdService _adService;

        public AdController(JWTService jwtService, AdService adService)
        {
            _jwtService = jwtService;
            _adService = adService;
        }

        [HttpGet]
        public IActionResult DisplayAllAds()
        {
            List<AdResponseDTO> adResponseDTOs = _adService.DisplayAll();
            if(adResponseDTOs != null){
                return Ok(adResponseDTOs);
            }
            return BadRequest("LeMauvaisCoin ne dispose d'aucune annonce");
        }

        [HttpPost("create")]
        public IActionResult CreateAd([FromBody] AdRequestDTO adRequest)
        {
            if (_adService.Create(adRequest) != null)
                return Ok("Annonce ajoutée avec succès");
            return BadRequest("Erreur lors de la création de l'annonce");
        }

        [HttpGet("{word}")]
        public IActionResult FindByKeyword(string word)
        {
            List<AdResponseDTO> adResponses = _adService.SearchByKeyword(word); 
            if (adResponses != null)
            {
                return Ok(adResponses);
            }
            return BadRequest("Aucun résultat trouvé, désolé :'(");
        }

        [HttpPost("{id}/comment")]
        public IActionResult AddComment(int id, [FromForm] string comment)
        {
            CommentResponseDTO comResDTO = _adService.AddComment(id, comment);
            if (comResDTO != null)
            {
                return Ok("Commentaire enregistré !");
            }
            return BadRequest("Pas de vilain commentaire aujourd'hui, sorry bro");
        }

        [HttpPost("{id}/picture")]
        public IActionResult AddPicture(int id, [FromForm] string url)
        {
            if (_adService.AddPicture(id, url))
            {
                return Ok("Image ajoutée avec succès");
            }
            return BadRequest("Erreur lors de l'ajout de l'image");
        }
    }
}
