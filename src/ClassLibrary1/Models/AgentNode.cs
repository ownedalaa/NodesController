using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class AgentNode
    {
        [Key]
        public int Id { get; set; }
        public string NodeId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Secret { get; set; } = "";
    }
}
