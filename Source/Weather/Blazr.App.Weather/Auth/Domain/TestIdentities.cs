/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Security.Claims;

namespace Blazr.App.Auth.Core;

public static class TestIdentities
{
    public const string Provider = "Dumb Provider";

    public static ClaimsIdentity GetIdentity(string userName)
    {
        var identity = Identities.FirstOrDefault(item => item.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (identity == null)
            return new ClaimsIdentity();

        return new ClaimsIdentity(identity.Claims, Provider);
    }

    public static ClaimsIdentity GetIdentity(Guid token)
    {
        var identity = Identities.FirstOrDefault(item => item.Id == token);
        if (identity == null)
            return new ClaimsIdentity();

        return new ClaimsIdentity(identity.Claims, Provider);
    }

    public static bool TryGetIdentity(Guid token, out ClaimsIdentity claimsIdentity)
    {
        var identity = Identities.FirstOrDefault(item => item.Id == token);
        claimsIdentity = new ClaimsIdentity(identity?.Claims, Provider) ?? new();

        return identity is not null;
    }

    public static List<TestIdentity> Identities = new List<TestIdentity>()
        {
            Visitor1Identity,
            Visitor2Identity,
            User1Identity,
            User2Identity,
            Admin1Identity,
            Admin2Identity
        };

    public static List<string> GetTestIdentities()
    {
        var list = new List<string> { "None" };
        list.AddRange(Identities.Select(identity => identity.Name!).ToList());
        return list;
    }

    public static Dictionary<Guid, string> TestIdentitiesDictionary()
    {
        var list = new Dictionary<Guid, string>();
        Identities.ForEach(identity => list.Add(identity.Id, identity.Name));
        return list;
    }

    public static TestIdentity User1Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-100000000001"),
            Name = "User-1",
            Role = "UserRole"
        };

    public static TestIdentity User2Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-100000000002"),
            Name = "User-2",
            Role = "UserRole"
        };

    public static TestIdentity Visitor1Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-200000000001"),
            Name = "Visitor-1",
            Role = "VisitorRole"
        };

    public static TestIdentity Visitor2Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-200000000002"),
            Name = "Visitor-2",
            Role = "VisitorRole"
        };

    public static TestIdentity Admin1Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-300000000001"),
            Name = "Admin-1",
            Role = "AdminRole"
        };

    public static TestIdentity Admin2Identity
        => new TestIdentity
        {
            Id = new Guid("10000000-0000-0000-0000-300000000002"),
            Name = "Admin-2",
            Role = "AdminRole"
        };
}

public record TestIdentity
{
    public string Name { get; set; } = string.Empty;

    public Guid Id { get; set; } = Guid.Empty;

    public string Role { get; set; } = string.Empty;

    public Claim[] Claims
        => new[]{
            new Claim(ClaimTypes.Sid, this.Id.ToString()),
            new Claim(ClaimTypes.Name, this.Name),
            new Claim(ClaimTypes.Role, this.Role)
    };

}

