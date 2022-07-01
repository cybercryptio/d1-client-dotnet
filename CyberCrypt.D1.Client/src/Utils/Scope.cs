// Copyright 2020-2022 CYBERCRYPT

namespace CyberCrypt.D1.Client.Utils;

/// <summary>
/// The possible permission scopes.
/// </summary>
public enum Scope {
    /// <summary>
    /// Scope to read and decrypt objects.
    /// </summary>
    Read,
    /// <summary>
    /// Scope to create new and encrypt objects.
    /// </summary>
    Create,
    /// <summary>
    /// Scope for getting object permission.
    /// </summary>
    Index,
    /// <summary>
    /// Scope to manage object permissions.
    /// </summary>
    ObjectPermissions,
    /// <summary>
    /// Scope to update objects.
    /// </summary>
    Update,
    /// <summary>
    /// Scope to delete objects.
    /// </summary>
    Delete
}

internal static class ScopeMapper {
    internal static Protobuf.Scope GetServiceScope(this Scope scope) {
        switch (scope) {
            case Scope.Read:
                return Protobuf.Scope.Read;
            case Scope.Create:
                return Protobuf.Scope.Create;
            case Scope.Index:
                return Protobuf.Scope.Index;
            case Scope.ObjectPermissions:
                return Protobuf.Scope.Objectpermissions;
            case Scope.Update:
                return Protobuf.Scope.Update;
            case Scope.Delete:
                return Protobuf.Scope.Delete;
            default:
                throw new ArgumentException("Unsupported Scope");
        }
    }
}
