namespace WebApplication1.DTOs
{
    public class UsuarioDto
    {

        public int UsuariosID { get; set; }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public IFormFile Foto { get; set; }
    }

    public class GrupoDto
    {
        public string NomeGrupo { get; set; }
        public int ParticipantesMax { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataRevelacao { get; set; }
        public string Descricao { get; set; }
        public int ID_Administrador { get; set; }
        public bool SorteioRealizado { get; set; }
        public IFormFile Foto { get; set; }

        public List<AddParticipanteDto> ParticipantesGrupo { get; set; }
    }
    public class AddParticipanteDto
    {
        public int GrupoID { get; set; }
        public int UsuarioID { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }
        public string Foto { get; set; }
    }
    public class GrupoDto2
    {
        public string NomeGrupo { get; set; }
        public int ParticipantesMax { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataRevelacao { get; set; }
        public string Descricao { get; set; }
        public int ID_Administrador { get; set; }
        public bool SorteioRealizado { get; set; }
        public IFormFile Foto { get; set; }
    }

    public class UsuarioDto2
    {

        public int UsuariosID { get; set; }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public IFormFile Foto { get; set; }
    }

    public class AddParticipante
    {
        public int GrupoID { get; set; }
        public int UsuarioID { get; set; }


    }
    public class EnviarConviteDto
    {

        public int GrupoId { get; set; }
        public string Email { get; set; }
        public int ConviteID { get; internal set; }
    }


    public class UsarTokenConviteDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
    }


}
