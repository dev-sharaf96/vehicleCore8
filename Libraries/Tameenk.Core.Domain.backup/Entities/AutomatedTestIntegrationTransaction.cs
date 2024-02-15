using System;
using System.ComponentModel;

namespace Tameenk.Core.Domain.Entities
{
    public class AutomatedTestIntegrationTransaction : BaseEntity
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string InputParams { get; set; }
        public string OutputParams { get; set; }
        public int StatusId { get; set; }
        public DateTime Date { get; set; }
        [DefaultValue(false)]
        public bool Retrieved { get; set; }
    }
}
