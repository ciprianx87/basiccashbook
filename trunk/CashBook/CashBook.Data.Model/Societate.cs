//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashBook.Data.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Societate
    {
        public Societate()
        {
            this.RegistreCasa = new HashSet<RegistruCasa>();
        }
    
        public int Id { get; set; }
        public string Nume { get; set; }
        public string Adresa { get; set; }
        public string CUI { get; set; }
    
        public virtual ICollection<RegistruCasa> RegistreCasa { get; set; }
    }
}
