namespace CapstoneProjectBE.Models;

// Minimal skill-evidence record. HasCvEvidence is deliberately never changed by assessment submissions.
public class UserSkill
{
    public Guid UserSkillID { get; set; }
    public Guid UserID { get; set; }
    public Guid SkillID { get; set; }
    public bool HasCvEvidence { get; set; }
    public bool IsKnowledgeVerified { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
