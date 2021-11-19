using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Organisations;

public class OrganisationPricing
{
    public OrganisationPlanPricing Starter { get; init; }

    public OrganisationPlanPricing Pro { get; init; }

    public OrganisationPlanPricing Business { get; init; }

    public OrganisationBillingEvent LastBillingEvent { get; init; }
}
