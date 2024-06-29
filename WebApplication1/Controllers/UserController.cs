using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		[HttpPost]
		[Route("AddUser")]
		public IActionResult AddUser([FromForm] User user)
		{
			// Aqui você pode processar os dados do usuário recebidos, como salvar no banco de dados

			// Exemplo simples: apenas retornar os dados recebidos
			return Ok(user);
		}
	}

	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public IFormFile Photo { get; set; }
	}
}
