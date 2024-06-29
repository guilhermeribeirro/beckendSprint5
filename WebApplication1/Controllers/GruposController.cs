using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApplication1.Dados.EntityFramework;
using WebApplication1.DTOs;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GruposController : ControllerBase
    {
        private readonly Contexto _context;

        public GruposController(Contexto context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("AdicionarGrupo")]
        public async Task<IActionResult> AddGroup([FromForm] GrupoDto2 groupDto2)
        {
            if (ModelState.IsValid)



            {

                byte[] fotoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await groupDto2.Foto.CopyToAsync(memoryStream);
                    fotoBytes = memoryStream.ToArray();
                }
                var adminExists = await _context.Usuarios.AnyAsync(u => u.UsuariosID == groupDto2.ID_Administrador);
                if (!adminExists)
                {
                    return BadRequest("O administrador especificado não existe.");
                }

                var group = new Grupos
                {
                    NomeGrupo = groupDto2.NomeGrupo,
                    ParticipantesMax = groupDto2.ParticipantesMax,
                    Valor = groupDto2.Valor,
                    DataRevelacao = groupDto2.DataRevelacao,
                    Descricao = groupDto2.Descricao,
                    ID_Administrador = groupDto2.ID_Administrador,
                    SorteioRealizado = groupDto2.SorteioRealizado,
                    Foto = fotoBytes

                };

                if (groupDto2.Foto != null && groupDto2.Foto.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await groupDto2.Foto.CopyToAsync(ms);
                        group.Foto = ms.ToArray();
                    }
                }

                _context.Grupos.Add(group);
                await _context.SaveChangesAsync();
                return Ok(group);
            }
            return BadRequest(ModelState);
        }

        //[HttpPost]
        //[Route("AdicionarParticipantes")]
        //public async Task<IActionResult> AddParticipante([FromForm] AddParticipanteDto participanteDto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var grupoExists = await _context.Grupos.AnyAsync(g => g.GruposID == participanteDto.GrupoID);
        //        var usuarioExists = await _context.Usuarios.AnyAsync(u => u.UsuariosID == participanteDto.UsuarioID);

        //        if (!grupoExists)
        //        {
        //            return BadRequest("O grupo especificado não existe.");
        //        }

        //        if (!usuarioExists)
        //        {
        //            return BadRequest("O usuário especificado não existe.");
        //        }

        //        var participanteGrupo = new ParticipantesGrupo
        //        {
        //            ID_Grupo = participanteDto.GrupoID,
        //            ID_Participante = participanteDto.UsuarioID
        //        };

        //        _context.ParticipantesGrupo.Add(participanteGrupo);
        //        await _context.SaveChangesAsync();
        //        return Ok(participanteGrupo);
        //    }
        //    return BadRequest(ModelState);
        //}

        [HttpGet]
        [Route("TodosGrupos")]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _context.Grupos.ToListAsync();
            return Ok(groups);
        }




        [HttpDelete]
        [Route("DeletarGrupo/{id}")]
        public async Task<IActionResult> DeleteGroups(int id)
        {
            var group = await _context.Grupos.FindAsync(id);
            if (group == null)
            {
                return NotFound(new { Message = "Grupo nao encontrado" });
            }

            _context.Grupos.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Grupo deletado com sucesso" });
        }


        [HttpGet("{id}")]
        public ActionResult<GrupoDto> GetGrupo(int id)
        {
            var grupo = _context.Grupos
                .Include(g => g.ParticipantesGrupo)
                .ThenInclude(p => p.Usuarios) // Inclua os dados do usuário se necessário
                .FirstOrDefault(g => g.GruposID == id);

            if (grupo == null)
            {
                return NotFound();
            }



            var grupoDto = new GrupoDto
            {
                NomeGrupo = grupo.NomeGrupo,
                ParticipantesMax = grupo.ParticipantesMax,
                Valor = grupo.Valor,
                DataRevelacao = grupo.DataRevelacao,
                Descricao = grupo.Descricao,
                ID_Administrador = grupo.ID_Administrador,
                SorteioRealizado = grupo.SorteioRealizado,
                Foto = null, // Adicione lógica para manipular a foto se necessário
                ParticipantesGrupo = grupo.ParticipantesGrupo.Select(p => new AddParticipanteDto
                {
                    GrupoID = p.ID_Grupo,
                    UsuarioID = p.ID_Participante,
                    Nome = p.Usuarios.Nome,
                    Foto = p.Usuarios.Foto != null ? Convert.ToBase64String(p.Usuarios.Foto) : null
                }).ToList()
            };

            // Manipulação de foto se necessário


            return Ok(grupoDto);
        }




        [HttpGet("nome/{nomeGrupo}")]
        public ActionResult<GrupoDto> GetGrupoPorNome(string nomeGrupo)
        {
            var grupo = _context.Grupos
                .Include(g => g.ParticipantesGrupo)
                .ThenInclude(pg => pg.Usuarios)
                .FirstOrDefault(g => g.NomeGrupo == nomeGrupo);

            if (grupo == null)
            {
                return NotFound();
            }

            var grupoDto = new GrupoDto
            {
                NomeGrupo = grupo.NomeGrupo,
                ParticipantesMax = grupo.ParticipantesMax,
                Valor = grupo.Valor,
                DataRevelacao = grupo.DataRevelacao,
                Descricao = grupo.Descricao,
                ID_Administrador = grupo.ID_Administrador,
                SorteioRealizado = grupo.SorteioRealizado,
                Foto = null,
                ParticipantesGrupo = grupo.ParticipantesGrupo.Select(p => new AddParticipanteDto
                {
                    GrupoID = p.ID_Grupo,
                    UsuarioID = p.ID_Participante,
                    Nome = p.Usuarios.Nome,
                    Email = p.Usuarios.Email,
                    Foto = p.Usuarios.Foto != null ? Convert.ToBase64String(p.Usuarios.Foto) : null
                }).ToList()
            };

            return Ok(grupoDto);
        }


        // POST: api/Grupos/AdicionarParticipante
        [HttpPost("AdicionarParticipante")]
        public async Task<IActionResult> AddParticipante([FromBody] AddParticipanteDto participanteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grupo = await _context.Grupos.FindAsync(participanteDto.GrupoID);
            var usuario = await _context.Usuarios.FindAsync(participanteDto.UsuarioID);

            if (grupo == null)
            {
                return BadRequest("O grupo especificado não existe.");
            }

            if (usuario == null)
            {
                return BadRequest("O usuário especificado não existe.");
            }

            var participanteGrupo = new ParticipantesGrupo
            {
                ID_Grupo = participanteDto.GrupoID,
                ID_Participante = participanteDto.UsuarioID
            };

            _context.ParticipantesGrupo.Add(participanteGrupo);
            await _context.SaveChangesAsync();

            return Ok(participanteGrupo);
        }


        [HttpPost("AdicionarParticipante2")]
        public async Task<IActionResult> AddParticipante([FromBody] AddParticipante participanteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grupo = await _context.Grupos.FindAsync(participanteDto.GrupoID);
            var usuario = await _context.Usuarios.FindAsync(participanteDto.UsuarioID);

            if (grupo == null)
            {
                return BadRequest("O grupo especificado não existe.");
            }

            if (usuario == null)
            {
                return BadRequest("O usuário especificado não existe.");
            }

            var participanteGrupo = new ParticipantesGrupo
            {
                ID_Grupo = participanteDto.GrupoID,
                ID_Participante = participanteDto.UsuarioID
            };

            _context.ParticipantesGrupo.Add(participanteGrupo);
            await _context.SaveChangesAsync();

            return Ok(participanteGrupo);
        }

        // GET: api/Grupos/5/Participantes
        [HttpGet("{id}/Participantes")]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetParticipantes(int id)
        {
            var grupo = await _context.Grupos
                .Include(g => g.ParticipantesGrupo)
                .ThenInclude(pg => pg.Usuarios)
                .FirstOrDefaultAsync(g => g.GruposID == id);

            if (grupo == null)
            {
                return NotFound();
            }

            return grupo.ParticipantesGrupo.Select(pg => pg.Usuarios).ToList();
        }

    }

    public class Grupos
    {
        public int GruposID { get; set; }
        public string NomeGrupo { get; set; }
        public int ParticipantesMax { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataRevelacao { get; set; }
        public string Descricao { get; set; }
        public int ID_Administrador { get; set; }
        public bool SorteioRealizado { get; set; }

        public byte[] Foto { get; set; }

        public ICollection<ParticipantesGrupo> ParticipantesGrupo { get; set; }
    }

    public class ParticipantesGrupo
    {
        public int ID_Grupo { get; set; }
        public int ID_Participante { get; set; }

        public Grupos Grupos { get; set; }

        public Usuarios Usuarios { get; set; }
    }
}
