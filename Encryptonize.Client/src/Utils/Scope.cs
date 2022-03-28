// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.Client.Utils;

public enum Scope {
    Read,
    Create,
    Index,
    ObjectPermissions,
    UserManagement,
    Update,
    Delete
}

public static class ScopeMapper {
    internal static Scopes.Scope GetServiceScope(this Scope scope) {
        switch (scope) {
            case Scope.Read:
                return Scopes.Scope.Read;
            case Scope.Create:
                return Scopes.Scope.Create;
            case Scope.Index:
                return Scopes.Scope.Index;
            case Scope.ObjectPermissions:
                return Scopes.Scope.Objectpermissions;
            case Scope.UserManagement:
                return Scopes.Scope.Usermanagement;
            case Scope.Update:
                return Scopes.Scope.Update;
            case Scope.Delete:
                return Scopes.Scope.Delete;
            default:
                throw new ArgumentException("Unsupported Scope");
        }
    }
}
