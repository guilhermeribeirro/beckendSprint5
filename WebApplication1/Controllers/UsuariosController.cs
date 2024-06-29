using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Dados.EntityFramework;
using System;
using WebApplication1.DTOs;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly Contexto _context;

        public UsuariosController(Contexto context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("AdicionarUsuario")]
        public async Task<IActionResult> AddUser([FromForm] UsuarioDto2 userDTO2)
        {
            if (ModelState.IsValid)
            {
                // Convertendo a imagem em bytes
                byte[] fotoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await userDTO2.Foto.CopyToAsync(memoryStream);
                    fotoBytes = memoryStream.ToArray();
                }

                // Criando um novo objeto Usuarios com os dados e a foto
                Usuarios user = new Usuarios
                {
                    Nome = userDTO2.Nome,
                    Email = userDTO2.Email,
                    Senha = userDTO2.Senha,
                    Foto = fotoBytes
                };

                // Salvar no banco de dados
                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            return BadRequest(ModelState);
        }





        [HttpPost]
        [Route("AdicionarParticipantes")]
        public async Task<IActionResult> AddParticipante([FromForm] AddParticipanteDto participanteDto)
        {
            if (ModelState.IsValid)
            {
                var grupoExists = await _context.Grupos.AnyAsync(g => g.GruposID == participanteDto.GrupoID);
                var usuarioExists = await _context.Usuarios.AnyAsync(u => u.UsuariosID == participanteDto.UsuarioID);

                if (!grupoExists)
                {
                    return BadRequest("O grupo especificado não existe.");
                }

                if (!usuarioExists)
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
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("TodosUsuarios")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Usuarios.ToListAsync();
            return Ok(users);
        }


        [HttpDelete]
        [Route("DeletarUsuario/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "Usuario nao encontrado" });
            }

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario deletado com sucesso" });
        }

        [HttpGet("Login")]
        public IActionResult Login([FromQuery] string email, [FromQuery] string senha)
        {
            var user = _context.Usuarios.SingleOrDefault(u => u.Email == email && u.Senha == senha);
            if (user == null)
            {
                return BadRequest("Usuário ou senha inválidos");
            }
            return Ok(user);
        }




    }

    public class Usuarios
    {
        public int UsuariosID { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public byte[] Foto { get; set; }

        public ICollection<ParticipantesGrupo> Grupos { get; set; }
    }


}
