using System.Net;

namespace Enclave.Sdk.Api.Data.EnrolmentKeys;

/// <summary>
/// Input model for IP Address constraints.
/// </summary>
public class EnrolmentKeyIpConstraintInput
{
    /// <summary>
    /// Constructor that takes a range as a CIDR notation.
    /// </summary>
    /// <param name="cidrNotation">The IP range as a CIDR notation.</param>
    public EnrolmentKeyIpConstraintInput(string cidrNotation)
    {
        if (string.IsNullOrWhiteSpace(cidrNotation))
        {
            throw new ArgumentNullException(nameof(cidrNotation));
        }

        if (!cidrNotation.Contains('/', StringComparison.Ordinal))
        {
            throw new ArgumentException("Incorrect CIDR Format");
        }

        if (!IPNetwork.TryParse(cidrNotation, out var network))
        {
            throw new ArgumentException("Incorrect CIDR Format");
        }

        Range = cidrNotation;
    }

    /// <summary>
    /// Constructor that takes an IPAddress object to fill the Range field.
    /// </summary>
    /// <param name="ipAddress">The IP Address to use as the Range.</param>
    public EnrolmentKeyIpConstraintInput(IPAddress ipAddress)
    {
        if (ipAddress is null)
        {
            throw new ArgumentNullException(nameof(ipAddress));
        }

        Range = ipAddress.ToString();
    }

    /// <summary>
    /// Constructor that takes two IPAddress objects to fill the Range property.
    /// </summary>
    /// <param name="startAddress">The start IP Address.</param>
    /// <param name="endAddress">The end IP Address.</param>
    public EnrolmentKeyIpConstraintInput(IPAddress startAddress, IPAddress endAddress)
    {
        if (startAddress is null)
        {
            throw new ArgumentNullException(nameof(startAddress));
        }

        if (endAddress is null)
        {
            throw new ArgumentNullException(nameof(endAddress));
        }

        Range = $"{startAddress} - {endAddress}";
    }

    /// <summary>
    /// The IP range.
    /// </summary>
    public string Range { get; } = default!;

    /// <summary>
    /// A description for the range.
    /// </summary>
    public string? Description { get; set; }
}