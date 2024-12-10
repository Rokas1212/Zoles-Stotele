using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stotele.Server.Models
{
    public class ProfilisDTO
    {
        public int Id { get; set; }
        public string Lytis { get; set; }
        public string ElektroninisPastas { get; set; }
        public string Vardas { get; set; }
        public string Slapyvardis { get; set; }
        public string Pavarde { get; set; }
        public string Role { get; set; } // "Administratorius", "Klientas", "Vadybininkas"
        
        // Fields for Klientas:
        public int? PastoKodas { get; set; }
        public string Miestas { get; set; }
        public DateTime? GimimoData { get; set; }
        public string Adresas { get; set; }

        // Fields for Vadybininkas:
        public string Skyrius { get; set; }
        public int? ParduotuveId { get; set; }
    }

}