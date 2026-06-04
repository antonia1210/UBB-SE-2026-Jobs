namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Skills")]
    public class Skill
    {
        [Key]
        [Column("SkillId")]
        public int SkillId { get; set; }

        [Column("Name", TypeName = "nvarchar(100)")]
        public string SkillName { get; set; } = string.Empty;

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
