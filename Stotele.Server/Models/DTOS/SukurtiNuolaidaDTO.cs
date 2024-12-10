using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stotele.Server.Models
{
    public class SukurtiNuolaidaDTO
    {
        public int Procentai { get; set; }
        public DateTime GaliojimoPabaiga { get; set; } 
        public int PrekeId { get; set; }
        public int? UzsakymasId { get; set; }        
    }
}