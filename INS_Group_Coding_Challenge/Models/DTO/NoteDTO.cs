using System.ComponentModel.DataAnnotations;

namespace INS_Group_Coding_Challenge.Models.DTO
{
    public class NoteDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Content { get; set; }
    }
}
