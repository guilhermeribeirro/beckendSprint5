using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dados.EntityFramework;
using WebApplication1.DTOs;
using WebApplication1.Interfaces;
using WebApplication1.Services.EmailService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvitesController : ControllerBase
    {
        private readonly Contexto _context;

        private readonly
            IEmailService _emailService;

        public ConvitesController(Contexto context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("EnviarConvite")]
        public async Task<IActionResult> EnviarConvite([FromBody] EnviarConviteDto conviteDto)
        {
            // Valide o grupo
            var grupo = await _context.Grupos.FindAsync(conviteDto.GrupoId);
            if (grupo == null)
            {
                return BadRequest("Grupo não encontrado.");
            }

            // Gere um token de convite
            var token = Guid.NewGuid().ToString();
            var convite = new Convite
            {
                ConviteID = conviteDto.ConviteID,
                GrupoID = conviteDto.GrupoId,
                Email = conviteDto.Email,
                Token = token,
                DataExpiracao = DateTime.UtcNow.AddDays(7),
                Usado = false
            };
            _context.Convites.Add(convite);
            await _context.SaveChangesAsync();

            // Envie o e-mail
            var linkConvite = $"https://seusite.com/convite/{token}";
            var mensagem = $"Você foi convidado para se juntar ao grupo {grupo.NomeGrupo}. Clique no link para aceitar o convite: {linkConvite}";

            await _emailService.SendEmailAsync(conviteDto.Email, "Convite para Grupo", mensagem);

            return Ok("Convite enviado com sucesso.");
        }
    }
}
