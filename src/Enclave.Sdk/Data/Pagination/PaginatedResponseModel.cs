using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.Sdk.Api.Data.Pagination;

/// <summary>
/// Response model for paginated data.
/// </summary>
/// <typeparam name="TItemType">The item type.</typeparam>
public class PaginatedResponseModel<TItemType>
{
    /// <summary>
    /// Metadata for the paginated data.
    /// </summary>
    public PaginationMetadata Metadata { get; init; }

    /// <summary>
    /// The related links for the current page of data.
    /// </summary>
    public PaginationLinks Links { get; init; }

    /// <summary>
    /// The requested page of items.
    /// </summary>
    public IEnumerable<TItemType> Items { get; init; }
}