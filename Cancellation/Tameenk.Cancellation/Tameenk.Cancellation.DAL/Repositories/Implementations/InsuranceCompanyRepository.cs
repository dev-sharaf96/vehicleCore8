using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tameenk.Cancellation.DAL.Entities;
using Tameenk.Cancellation.DAL.Repositories.Interfaces;

namespace Tameenk.Cancellation.DAL.Repositories
{

    public class InsuranceCompanyRepository : GRUDRepository<InsuranceCompany>, IInsuranceCompanyRepository
    {
        public InsuranceCompanyRepository(DbContext context) : base(context)
        { }

        private CancellationContext _appContext => (CancellationContext)_context;
    }

}
