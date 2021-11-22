// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "We are using variables for tests they do not need to be disposed at end of scope", Scope = "member", Target = "~M:Enclave.Sdk.Api.Tests.Clients.OrganisationClientTests.Setup")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "unit test methods use underscores for readability", Scope = "member", Target = "~M:Enclave.Sdk.Api.Tests.Clients.OrganisationClientTests.Should_return_a_detailed_organisation_model_when_calling_GetAsync~System.Threading.Tasks.Task")]
