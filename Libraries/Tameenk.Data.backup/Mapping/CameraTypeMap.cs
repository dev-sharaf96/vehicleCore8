﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Data.Mapping
{
    class CameraTypeMap : EntityTypeConfiguration<CameraType>
    {
        public CameraTypeMap()
        {
            ToTable("CameraType");
            HasKey(e => e.Id);
            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

    }
}