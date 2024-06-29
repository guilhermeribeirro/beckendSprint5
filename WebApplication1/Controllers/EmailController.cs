using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Dados.EntityFramework;
using WebApplication1.DTOs;
using WebApplication1.Interfaces;
//using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly Contexto _context;
        private readonly IEmailService _emailService;

        public EmailController(Contexto context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("EnviarConvite")]
        public async Task<IActionResult> EnviarConvite([FromBody] EnviarConviteDto conviteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grupo = await _context.Grupos.FindAsync(conviteDto.GrupoId);
            if (grupo == null)
            {
                return BadRequest("O grupo especificado não existe.");
            }

            var token = Guid.NewGuid().ToString();
            var dataExpiracao = DateTime.UtcNow.AddDays(7); // Token válido por 7 dias

            var convite = new Convite
            {
                GrupoID = conviteDto.GrupoId,
                Email = conviteDto.Email,
                Token = token,
                DataExpiracao = dataExpiracao,
                Usado = false
            };

            _context.Convites.Add(convite);
            await _context.SaveChangesAsync();

            var linkConvite = $"https://seusite.com/convite?token={token}";
            await _emailService.SendEmailAsync(conviteDto.Email, "Convite para Grupo", $"Você foi convidado para participar de um grupo. Clique no link para se cadastrar: {linkConvite}");

            return Ok("Convite enviado com sucesso.");
        }


        [HttpGet("VerificarToken")]
        public async Task<IActionResult> VerificarToken([FromQuery] string token)
        {
            var convite = await _context.Convites.FirstOrDefaultAsync(c => c.Token == token && !c.Usado && c.DataExpiracao > DateTime.UtcNow);
            if (convite == null)
            {
                return BadRequest("Token inválido ou expirado.");
            }

            return Ok(new { isValid = true });
        }


        [HttpPost("UsarTokenConvite")]
        public async Task<IActionResult> UsarTokenConvite([FromBody] UsarTokenConviteDto usarTokenDto)
        {
            var convite = await _context.Convites.FirstOrDefaultAsync(c => c.Token == usarTokenDto.Token && !c.Usado && c.DataExpiracao > DateTime.UtcNow);
            if (convite == null)
            {
                return BadRequest("Token inválido ou expirado.");
            }

            var usuario = await _context.Usuarios.FindAsync(usarTokenDto.UserId);
            if (usuario == null)
            {
                return BadRequest("Usuário não encontrado.");
            }

            var participanteGrupo = new ParticipantesGrupo
            {
                ID_Grupo = convite.GrupoID,
                ID_Participante = usarTokenDto.UserId
            };

            _context.ParticipantesGrupo.Add(participanteGrupo);
            convite.Usado = true;
            await _context.SaveChangesAsync();

            return Ok("Usuário adicionado ao grupo com sucesso.");
        }



    }


    public class Convite
    {

        public int Id { get; set; }
        public int ConviteID { get; set; }
        public int GrupoID { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime DataExpiracao { get; set; }
        public bool Usado { get; set; }
    }


}
