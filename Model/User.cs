using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerApplication.Model {
    [Table("user")]
    public class User {
        [Column("id")]
        public int Id { get; set; }

        [Column("tenant")]
        public string Tenant { get; set; } = "";

        [Column("username")]
        public string Username { get; set; } = "";

        [Column("company")]
        public string Company { get; set; } = "";

        [Column("key")]
        public string Key { get; set; } = "";

        [Column("hash")]
        public string Hash { get; set; } = "";

    }
}
