// Copyright 2020-2022 CYBERCRYPT

namespace CyberCrypt.D1.Client.Utils;

/// <summary>
/// The possible permission scopes.
/// </summary>
public enum Scope
{
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
    GetAccess,
    /// <summary>
    /// Scope to modify object permissions.
    /// </summary>
    ModifyAccess,
    /// <summary>
    /// Scope to update objects.
    /// </summary>
    Update,
    /// <summary>
    /// Scope to delete objects.
    /// </summary>
    Delete,
    /// <summary>
    /// Scope to use secure index for searching in data.
    /// </summary>
    Index
}

internal static class ScopeMapper
{
    internal static Protobuf.Scopes.Scope GetServiceScope(this Scope scope)
    {
        switch (scope)
        {
            case Scope.Read:
                return Protobuf.Scopes.Scope.Read;
            case Scope.Create:
                return Protobuf.Scopes.Scope.Create;
            case Scope.GetAccess:
                return Protobuf.Scopes.Scope.Getaccess;
            case Scope.ModifyAccess:
                return Protobuf.Scopes.Scope.Modifyaccess;
            case Scope.Update:
                return Protobuf.Scopes.Scope.Update;
            case Scope.Delete:
                return Protobuf.Scopes.Scope.Delete;
            case Scope.Index:
                return Protobuf.Scopes.Scope.Index;
            default:
                throw new ArgumentException("Unsupported Scope");
        }
    }
}
