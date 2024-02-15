namespace Tameenk.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Logger")]
    public partial class Logger
    {
        public Guid Id { get; set; }

        public string StackTrace { get; set; }

        public string Message { get; set; }

        public string ExceptionAsString { get; set; }

        public DateTime? ExceptionDate { get; set; }

        public string key { get; set; }

        public int? Level { get; set; }
    }
}
