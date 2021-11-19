using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Organisations;

public class Organisation : IDataModel
{
    // TODO shall we use a organisationGuid?
    public string Id { get; init; }

    public DateTime Created { get; init; }

    public string Name { get; init; }

    // TODO DO we want an enum here?
    public string Plan { get; init; }

    public string? Website { get; init; }

    public string? Phone { get; init; }

    public int MaxSystems { get; init; }

    public long EnrolledSystems { get; init; }

    public long UnapprovedSystems { get; init; }
}